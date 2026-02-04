using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{

   
    new public string name = "New Item";
    public Sprite icon = null;
    public bool isBreakable = false;
    public Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 position = new Vector3(0.5f, 0.1f, 0.0f);
    public bool isDefaultItem = false;
    public float attackRate = 1;

    public int strengthModifier = 0;
    public int defenceModifier = 0;
    public int speedModifier = 0;
    public int glowModifier = 0;

    public int itemID = -1;


}
