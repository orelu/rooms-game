using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestLogic : MonoBehaviour
{
    private BoxCollider2D collider;
    private SpriteRenderer spriteRenderer;

    public Sprite tileUnselected;
    public Sprite tileSelected;
    public Sprite tileOpened;

    public List<Item> items;
    public bool chestOpened;

    [HideInInspector] public GameObject player;        // auto-assigned
    [HideInInspector] public GameObject chestUI;       // auto-assigned (child of MainCanvas)
    [HideInInspector] public GameObject hotbar;        // auto-assigned (MainCanvas/Hotbar/Hotbar)
    [HideInInspector] public GameObject localLight;    // auto-assigned (child of self)
    [HideInInspector] public InventoryUI inventoryUI;  // auto-assigned (child of MainCanvas)
    [HideInInspector] public Transform itemsParent;    // auto-assigned (MainCanvas/ChestUI/Panel)

    public int radius = 5;
    public LootTable lootTable;

    private ChestSlot[] slots;

    void Awake()
    {
        // Auto-assign key references
        player = GameObject.Find("Player");

        GameObject mainCanvas = GameObject.Find("MainCanvas");
        if (mainCanvas != null)
        {
            chestUI = mainCanvas.transform.Find("ChestUI")?.gameObject;
            hotbar = mainCanvas.transform.Find("Hotbar/Hotbar")?.gameObject;
            inventoryUI = mainCanvas.GetComponentInChildren<InventoryUI>(true);

            // Find "Panel" under ChestUI
            if (chestUI != null) {
                Transform subChestUI = chestUI.transform.Find("ChestUI");
                if (subChestUI!=null)
                    itemsParent = subChestUI.Find("Panel");
            }
                
        }

        // Light child of self
        Transform lightChild = transform.Find("chestLight");
        if (lightChild != null)
            localLight = lightChild.gameObject;

        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = tileUnselected;
        chestOpened = false;
    }

    void Start()
    {
        if (lootTable != null)
        {
            items = lootTable.GetAllDroppedItems();
        }
    }

    public void openChest()
    {
        chestOpened = true;
        chestUI.SetActive(true);
        hotbar.SetActive(false);
        Time.timeScale = 0;

        ItemLogic.instance.interactingObject = gameObject;
        slots = itemsParent.GetComponentsInChildren<ChestSlot>();

        inventoryUI.ChestLogic = this;
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].chest = this;
            if (i < items.Count)
            {
                slots[i].AddItem(items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    public void closeChest()
    {
        spriteRenderer.sprite = tileUnselected;
        chestOpened = false;

        chestUI.SetActive(false);
        hotbar.SetActive(true);
        Time.timeScale = 1;

        ItemLogic.instance.interactingObject = null;

        if (localLight != null)
            localLight.SetActive(false);
    }

    public void removeItem(Item removedItem)
    {
        items.Remove(removedItem);
    }

    void OnMouseEnter()
    {
        if (!chestOpened && Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            spriteRenderer.sprite = tileSelected;
            if (localLight != null)
                localLight.SetActive(true);
            ItemLogic.instance.interactingObject = gameObject;
        }
    }

    void OnMouseExit()
    {
        if (!chestOpened)
        {
            spriteRenderer.sprite = tileUnselected;
            if (localLight != null)
                localLight.SetActive(false);
            ItemLogic.instance.interactingObject = null;
        }
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1) && Vector3.Distance(transform.position, player.transform.position) < radius && !chestOpened)
        {
            spriteRenderer.sprite = tileOpened;
            openChest();
        }
    }
}
