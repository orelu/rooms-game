using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [HideInInspector] public Image icon;
    [HideInInspector] public Button removeButton;
    [SerializeField] private Image highlightImage; // Assign in Inspector (transparent highlight overlay)
    
    Item item;
    [HideInInspector] public Animator playerAnimator;

    private bool isHovering = false;

    void Awake()
    {
        // Find "itemButton/icon" for icon
        Transform itemButton = transform.Find("ItemButton");
        if (itemButton != null)
        {
            Transform iconTransform = itemButton.Find("Icon");
            if (iconTransform != null)
                icon = iconTransform.GetComponent<Image>();
        }

        // Find "Close" button directly under this GameObject
        Transform closeTransform = transform.Find("CloseButton");
        if (closeTransform != null)
            removeButton = closeTransform.GetComponent<Button>();

        // Find the player and get its Animator
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerAnimator = player.GetComponent<Animator>();

        // Ensure highlight is hidden at start
        if (highlightImage != null)
            highlightImage.enabled = false;
    }

    void Update()
    {
        // Only highlight if hovering AND left mouse button is held
        if (highlightImage != null)
            highlightImage.enabled = isHovering && Input.GetMouseButton(0);
    }

    public void AddItem(Item newItem)
    {
        item = newItem;

        if (icon != null)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }

        if (removeButton != null)
            removeButton.interactable = true;
    }

    public void ClearSlot()
    {
        item = null;

        if (icon != null)
        {
            icon.sprite = null;
            icon.enabled = false;
        }

        if (removeButton != null)
            removeButton.interactable = false;
    }

    public void onRemoveButtonPressed()
    {
        if (item != null)
            Inventory.instance.Remove(item);
    }

    public void UseItem()
    {
        if (item != null)
        {
            ItemLogic.instance.Use(1, item);

            if (playerAnimator != null)
                playerAnimator.SetTrigger("UseItem");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
