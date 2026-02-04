using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Loot Table", menuName = "Inventory/LootTable")]
public class LootTable : ScriptableObject
{

    public List<LootEntry> lootEntries;

    public bool AddItem(LootEntry entry) {
        float r = Random.value;
        if (r <= entry.dropChance)
        {
            return true;
        } else {
            return false;
        }
    }

    public List<Item> GetAllDroppedItems()
    {
        List<Item> droppedItems = new List<Item>();

        

        foreach (var entry in lootEntries)
        {
            for (int i=0;i<entry.maxNumber;i++) {
                if (AddItem(entry))
                {
                    droppedItems.Add(entry.item);
                }
            }
        }

        foreach (var item in droppedItems)
        {
            Debug.Log("Dropped: " + item.name);
        }

        return droppedItems;
    }


}
