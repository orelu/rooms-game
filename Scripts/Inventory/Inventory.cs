using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    
    public delegate void OnItemChanged();

    public OnItemChanged onItemChangedCallback;


    public int inventorySize = 10;
    
    public List<Item> items;

    public static Inventory instance;

    [HideInInspector] public HotbarSelector HotbarSelector;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found.");
            return;
        }
        else
        {
            instance = this;
        }

        // Find Canvas
        GameObject canvas = GameObject.Find("MainCanvas");
        if (canvas == null)
        {
            Debug.LogWarning("Canvas not found in the scene.");
            return;
        }

        // Find Hotbar under Canvas
        Transform hotbar = canvas.transform.Find("Hotbar");
        if (hotbar == null)
        {
            Debug.LogWarning("Hotbar object not found as a child of Canvas.");
            return;
        }

        // Find HotbarSelector component under Hotbar (anywhere in children)
        HotbarSelector = hotbar.GetComponentInChildren<HotbarSelector>();
        if (HotbarSelector == null)
        {
            Debug.LogWarning("HotbarSelector component not found under Hotbar.");
        }
    }


    public Item getSelectedItem() {
        int slotSelected = HotbarSelector.slotSelected;
        List<Item> items = Inventory.instance.items;

        if (slotSelected < items.Count) {
            return items[slotSelected];
        } else {
            return null;
        }

        
    }

    public bool Add (Item item)
    {
        if (!item.isDefaultItem)
        {
            if (items.Count >= inventorySize)
            {
                return false;
            } else {
                items.Add(item);
                HotbarSelector.moveSelection(HotbarSelector.slotSelected);

                if (onItemChangedCallback != null)
                {
                    onItemChangedCallback.Invoke();
                }
            }
            
        }  

        return true;  
    }

    public void Remove (Item item)
    {
        items.Remove(item);
        HotbarSelector.moveSelection(HotbarSelector.slotSelected);

        if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexB < 0 || indexA >= items.Count || indexB >= items.Count)
            return;

        (items[indexA], items[indexB]) = (items[indexB], items[indexA]);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }


    


}
