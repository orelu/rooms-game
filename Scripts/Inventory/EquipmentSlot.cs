using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{

    [HideInInspector] public Image icon;
    public bool isSidebar = true;

    [HideInInspector] public Animator playerAnimator;
    
    [HideInInspector] public Button removeButton;
    
    Item item;

    public void AddItem(Item newItem)
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
            
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;

        if (!isSidebar) {removeButton.interactable = true;}
        
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        
        if (!isSidebar) {removeButton.interactable = false;}
    }

    public void onRemoveButtonPressed()
    {
        EquipmentInventory.instance.Remove(item);
    }

    public void UseItem()
    {
        if (item != null)
        {
            ItemLogic.instance.Use(1, item);
        }
    }

}
