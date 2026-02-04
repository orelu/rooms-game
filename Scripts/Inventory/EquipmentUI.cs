using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUI : MonoBehaviour
{
    public Transform itemsParent;

    EquipmentInventory inventory;

    EquipmentSlot[] slots;

    // Start is called before the first frame update
    void Start()
    {
        inventory = EquipmentInventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<EquipmentSlot>();

        UpdateUI();
    }


    void UpdateUI()
    {
        for (int i=0;i<4;i++)
        {
            if (i < inventory.items.Count)
            {
                slots[i].AddItem(inventory.items[i]);
            } else{
                slots[i].ClearSlot();
            }
        }
    }
}
