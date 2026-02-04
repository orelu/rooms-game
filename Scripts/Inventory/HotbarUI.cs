using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    [HideInInspector] public Transform itemsParent;

    Inventory inventory;
    HotbarSlot[] slots;

    void Awake()
    {
        // Auto-assign itemsParent by finding "Hotbar/Panel/*"
        if (itemsParent == null)
        {
            Transform hotbar = transform.Find("Hotbar");
            if (hotbar != null)
            {
                Transform panel = hotbar.Find("Panel");
                if (panel != null && panel.childCount > 0)
                {
                    itemsParent = panel.GetChild(0); // assumes first child is itemsParent
                }
                else
                {
                    Debug.LogWarning("HotbarUI: Panel or itemsParent not found.");
                }
            }
            else
            {
                Debug.LogWarning("HotbarUI: Hotbar not found.");
            }
        }
    }

    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        if (itemsParent != null)
            slots = itemsParent.GetComponentsInChildren<HotbarSlot>();

        UpdateUI();
    }

    void UpdateUI()
    {
        if (slots == null || slots.Length == 0)
            return;

        for (int i = 0; i < 6; i++)
        {
            if (i < inventory.items.Count)
                slots[i].AddItem(inventory.items[i]);
            else
                slots[i].ClearSlot();
        }
    }
}
