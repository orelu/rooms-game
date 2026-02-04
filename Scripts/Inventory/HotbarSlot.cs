using UnityEngine;
using UnityEngine.UI;

public class HotbarSlot : MonoBehaviour
{

    [HideInInspector] public Image icon;
    [HideInInspector] public Animator playerAnimator;
    Item item;

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

        // Find the player and get its Animator
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerAnimator = player.GetComponent<Animator>();
    }


    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void UseItem()
    {
        if (item != null)
        {
            ItemLogic.instance.Use(1, item);
        }
    }
}
