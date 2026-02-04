using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryUI : MonoBehaviour
{
    [HideInInspector] public Transform itemsParent;
    [HideInInspector] public GameObject inventoryUI;
    [HideInInspector] public GameObject hotbar;
    [HideInInspector] public GameObject chestUI;
    [HideInInspector] public GameObject bars;

    [HideInInspector] public ChestLogic ChestLogic;

    Inventory inventory;
    InventorySlot[] slots;

    // Hotkey swap: only when a slot is hovered AND LMB is held AND inventory is open
    private int hoveredSlotIndex = -1;

    // Drag & drop swap
    private int dragSourceIndex = -1;
    private CanvasGroup[] slotCanvasGroups; // to disable raycasts during drag

    void Start()
    {
        // Auto-find UI references based on hierarchy
        inventoryUI = transform.Find("Inventory").gameObject;

        if (transform.parent != null)
        {
            Transform siblingHotbar = transform.parent.Find("Hotbar");
            if (siblingHotbar != null)
            {
                Transform childHotbar = transform.Find("Hotbar");
                if (childHotbar != null)
                    hotbar = childHotbar.gameObject;
            }

            Transform siblingChest = transform.parent.Find("ChestUI");
            if (siblingChest != null)
                chestUI = siblingChest.gameObject;

            Transform siblingBars = transform.parent.Find("Bars");
            if (siblingBars != null)
                bars = siblingBars.gameObject;
        }

        itemsParent = inventoryUI.transform.Find("Panel");

        inventory = Inventory.instance;
        inventory.onItemChangedCallback += UpdateUI;

        slots = itemsParent.GetComponentsInChildren<InventorySlot>(true);

        // Prepare per-slot EventTriggers and CanvasGroups
        slotCanvasGroups = new CanvasGroup[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            int index = i;

            // Ensure a CanvasGroup exists so we can toggle raycast blocking while dragging
            var cg = slots[i].GetComponent<CanvasGroup>();
            if (cg == null) cg = slots[i].gameObject.AddComponent<CanvasGroup>();
            slotCanvasGroups[i] = cg;

            // Add EventTrigger for hover + drag/drop
            var trigger = slots[i].GetComponent<EventTrigger>();
            if (trigger == null) trigger = slots[i].gameObject.AddComponent<EventTrigger>();

            // Hover enter
            var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener((_) =>
            {
                if (IsInventoryOpen()) hoveredSlotIndex = index;
            });
            trigger.triggers.Add(enter);

            // Hover exit
            var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exit.callback.AddListener((_) =>
            {
                if (hoveredSlotIndex == index) hoveredSlotIndex = -1;
            });
            trigger.triggers.Add(exit);

            // Begin drag
            var beginDrag = new EventTrigger.Entry { eventID = EventTriggerType.BeginDrag };
            beginDrag.callback.AddListener((_) =>
            {
                if (!IsInventoryOpen()) return;
                dragSourceIndex = index;

                // Let raycasts pass through the dragged slot so the target receives OnDrop
                slotCanvasGroups[index].blocksRaycasts = false;
            });
            trigger.triggers.Add(beginDrag);

            // Drop (fires on the target slot)
            var drop = new EventTrigger.Entry { eventID = EventTriggerType.Drop };
            drop.callback.AddListener((_) =>
            {
                if (!IsInventoryOpen()) return;
                if (dragSourceIndex == -1) return;

                int targetIndex = index; // this trigger belongs to the drop target slot
                inventory.SwapItems(dragSourceIndex, targetIndex);
            });
            trigger.triggers.Add(drop);

            // End drag (fires on the source slot)
            var endDrag = new EventTrigger.Entry { eventID = EventTriggerType.EndDrag };
            endDrag.callback.AddListener((_) =>
            {
                // Re-enable raycasts so the slot is interactive again
                if (dragSourceIndex >= 0 && dragSourceIndex < slotCanvasGroups.Length)
                    slotCanvasGroups[dragSourceIndex].blocksRaycasts = true;

                dragSourceIndex = -1;
            });
            trigger.triggers.Add(endDrag);
        }

        UpdateUI();
    }

    public void close()
    {
        inventoryUI.SetActive(false);
        if (hotbar != null) hotbar.SetActive(true);
        Time.timeScale = 1;

        // Reset hover so number keys won't act on a stale index
        hoveredSlotIndex = -1;
    }

    void Update()
    {
        // Toggle inventory open/close
        if (Input.GetButtonDown("Inventory"))
        {
            bool isActive = inventoryUI.activeSelf;
            bool isChestUIActive = chestUI != null && chestUI.activeSelf;
            bars.SetActive(isActive);

            if (isActive)
            {
                inventoryUI.SetActive(false);
                if (hotbar != null) hotbar.SetActive(true);
                Time.timeScale = 1;

                // When closing, clear states so no accidental swaps
                hoveredSlotIndex = -1;
                dragSourceIndex = -1;

                // Make sure all slots accept raycasts again
                for (int i = 0; i < slotCanvasGroups.Length; i++)
                    if (slotCanvasGroups[i] != null) slotCanvasGroups[i].blocksRaycasts = true;
            }
            else if (!isChestUIActive)
            {
                inventoryUI.SetActive(true);
                if (hotbar != null) hotbar.SetActive(false);
                Time.timeScale = 0;
            }
            else
            {
                if (ChestLogic != null) ChestLogic.closeChest();
            }
        }

        // === Hotkey swap (ONLY when inventory open, mouse button held on a slot) ===
        if (IsInventoryOpen() && hoveredSlotIndex >= 0 && Input.GetMouseButton(0))
        {
            // Use KeyCode.Alpha1..Alpha6 for reliability
            if (Input.GetKeyDown(KeyCode.Alpha1)) TrySwapHotbar(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) TrySwapHotbar(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) TrySwapHotbar(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) TrySwapHotbar(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5)) TrySwapHotbar(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6)) TrySwapHotbar(5);
        }
    }

    private bool IsInventoryOpen()
    {
        return inventoryUI != null && inventoryUI.activeSelf;
    }

    private void TrySwapHotbar(int hotbarIndex)
    {
        // Only swap if both indices point to actual items (compact list)
        if (hoveredSlotIndex < 0) return;
        if (hotbarIndex < 0 || hotbarIndex >= inventory.items.Count) return;

        inventory.SwapItems(hoveredSlotIndex, hotbarIndex);
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.items.Count)
                slots[i].AddItem(inventory.items[i]);
            else
                slots[i].ClearSlot();
        }
    }
}
