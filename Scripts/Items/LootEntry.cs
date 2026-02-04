using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Loot Entry", menuName = "Inventory/LootEntry")]
public class LootEntry : ScriptableObject
{
    
    public Item item;
    [Range(0f, 1f)]
    public float dropChance; // 0.0 to 1.0, e.g., 0.25 = 25%
    public int maxNumber = 1;

}
