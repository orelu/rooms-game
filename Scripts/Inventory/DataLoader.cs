using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class DataLoader : MonoBehaviour
{
    [SerializeField] private List<Item> itemDatabase; // Assign in inspector
    [SerializeField] private PlayerStats playerStats; // Assign in inspector

    void Start()
    {
        LoadAllData();
    }

    public void LoadAllData()
    {
        LoadPlayerStats();
        LoadInventory();
    }

    public void LoadPlayerStats()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "PlayerStats.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerStatsData data = JsonUtility.FromJson<PlayerStatsData>(json);
            playerStats.FromData(data);
        }
    }

    public void LoadInventory()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "PlayerInventory.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            ItemDataListWrapper wrapper = JsonUtility.FromJson<ItemDataListWrapper>(json);
            
            Inventory.instance.items.Clear();
            
            foreach (ItemData data in wrapper.items)
            {
                Item item = ApplyItemData(data);
                Inventory.instance.items.Add(item);
            }
            Inventory.instance.onItemChangedCallback.Invoke();
        }
    }


    private Item ApplyItemData(ItemData data)
    {
        Item item = new Item();
        item.name = data.name;
        item.isBreakable = data.isBreakable;
        item.scale = data.scale;
        item.position = data.position;
        item.isDefaultItem = data.isDefaultItem;
        item.attackRate = data.attackRate;
        item.strengthModifier = data.strengthModifier;
        item.defenceModifier = data.defenceModifier;
        item.speedModifier = data.speedModifier;
        item.glowModifier = data.glowModifier;
        item.itemID = data.itemID;
        item.icon = data.icon;
        return item;
    }
}