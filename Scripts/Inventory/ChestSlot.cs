using UnityEngine;
using UnityEngine.UI;

public class ChestSlot : MonoBehaviour
{

    public Image icon;
    public Button addButton;
    public Button mainButton;
    public ChestLogic chest;

    Item item;

    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;
        icon.enabled = true;
        addButton.interactable = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        addButton.interactable = false;
    }

    public void onAddButtonPressed(Item item)
    {
        if (item != null)
        {
            Inventory.instance.Add(item);
            ClearSlot();
            chest.removeItem(item);
        }  

    }

    void Start()
    {
        if (addButton != null)
        {
            addButton.onClick.AddListener(() => onAddButtonPressed(item));
            mainButton.onClick.AddListener(() => onAddButtonPressed(item));
        }
    }


}
