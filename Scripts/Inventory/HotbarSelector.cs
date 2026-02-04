using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class HotbarSelector : MonoBehaviour
{
    public static int slotSelected = 0;
    public int totalSlots = 6;

    public float switchCooldown = 0.1f;
    private float nextSwitchTime = 0f;

    [HideInInspector] public RectTransform hotbarSelector;
    [HideInInspector] public Light2D playerLight;
    [HideInInspector] public GameObject itemHolder;
    [HideInInspector] public SpriteRenderer renderer;
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public GameObject itemNameDisplay;

    TextMeshProUGUI itemText;
    private Coroutine hideNameCoroutine;

    void Awake()
    {
        // Get component from self
        hotbarSelector = GetComponent<RectTransform>();

        // Find player object and its children
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerLight = player.GetComponentInChildren<Light2D>();
            itemHolder = player.transform.Find("ItemHolder")?.gameObject;

            if (itemHolder != null)
                renderer = itemHolder.GetComponent<SpriteRenderer>();
        }

        // Find PlayerStats in the scene (not parent-child related)
        playerStats = FindObjectOfType<PlayerStats>();

        // Get sibling text display object and its TextMeshPro component
        Transform parent = transform.parent;
        if (parent != null)
        {
            Transform sibling = parent.Find("ItemNameDisplay");
            if (sibling != null)
            {
                itemNameDisplay = sibling.gameObject;
                itemText = itemNameDisplay.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }

    void Start()
    {
        StartCoroutine(WaitForInventoryAndSelect());
        itemText.text = "";
    }

    IEnumerator WaitForInventoryAndSelect()
    {
        while (Inventory.instance == null || Inventory.instance.items == null)
            yield return null;

        while (Inventory.instance.items.Count == 0)
            yield return null;

        moveSelection(0);
    }

    IEnumerator HideItemNameAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        itemNameDisplay.SetActive(false);
    }

    public void moveSelection(int s)
    {
        if (Inventory.instance == null || Inventory.instance.items == null)
            return;

        List<Item> items = Inventory.instance.items;

        hotbarSelector.anchoredPosition = new Vector2((s * 68) - 170, 0);
        slotSelected = s;

        if (s < items.Count)
        {
            Item i = items[s];
            if (playerLight != null)
            {
                playerLight.intensity = 1.0f + i.glowModifier;
                playerLight.pointLightOuterRadius = 5.0f + i.glowModifier * 2;
            }

            if (itemHolder != null)
                itemHolder.transform.localScale = i.scale * 3;

            if (renderer != null)
                renderer.sprite = i.icon;

            if (playerStats != null)
            {
                playerStats.damage.setValue(1 + i.strengthModifier);
                playerStats.speed.setValue(1 + i.speedModifier);
                playerStats.armor.setValue(0 + i.defenceModifier);
            }

            if (itemNameDisplay != null && itemText != null)
            {
                itemNameDisplay.SetActive(true);
                itemText.text = string.IsNullOrEmpty(i.name) ? "" : i.name;

                if (gameObject.activeInHierarchy)
                {
                    if (hideNameCoroutine != null)
                        StopCoroutine(hideNameCoroutine);

                    hideNameCoroutine = StartCoroutine(HideItemNameAfterDelay());
                }
            }
        }
        else
        {
            if (playerLight != null)
            {
                playerLight.intensity = 1.0f;
                playerLight.pointLightOuterRadius = 5.0f;
            }

            if (itemHolder != null)
                itemHolder.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);

            if (renderer != null)
                renderer.sprite = null;

            if (playerStats != null)
            {
                playerStats.damage.setValue(1);
                playerStats.speed.setValue(1);
                playerStats.armor.setValue(0);
            }

            if (itemNameDisplay != null)
                itemNameDisplay.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (Inventory.instance == null || Inventory.instance.items == null)
            return;

        if (Input.GetKeyDown("1")) moveSelection(0);
        else if (Input.GetKeyDown("2")) moveSelection(1);
        else if (Input.GetKeyDown("3")) moveSelection(2);
        else if (Input.GetKeyDown("4")) moveSelection(3);
        else if (Input.GetKeyDown("5")) moveSelection(4);
        else if (Input.GetKeyDown("6")) moveSelection(5);

        if (Time.time >= nextSwitchTime)
        {
            float scroll = 0f;
            if (Input.GetKey(KeyCode.UpArrow)) scroll = 1f;
            else if (Input.GetKey(KeyCode.DownArrow)) scroll = -1f;

            int previousSlot = slotSelected;

            if (scroll > 0f)
            {
                slotSelected = (slotSelected + 1) % totalSlots;
                nextSwitchTime = Time.time + switchCooldown;
            }
            else if (scroll < 0f)
            {
                slotSelected = (slotSelected - 1 + totalSlots) % totalSlots;
                nextSwitchTime = Time.time + switchCooldown;
            }

            if (slotSelected != previousSlot)
            {
                moveSelection(slotSelected);
            }
        }
    }
}
