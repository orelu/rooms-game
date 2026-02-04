using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventory : MonoBehaviour
{
    
    public delegate void OnItemChanged();

    public OnItemChanged onItemChangedCallback;


    public int inventorySize = 4;
    
    public List<Item> items;

    public static EquipmentInventory instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Inventory found.");
            return;
        }

        instance = this;
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

        if (onItemChangedCallback != null)
            {
                onItemChangedCallback.Invoke();
            }
    }


}
