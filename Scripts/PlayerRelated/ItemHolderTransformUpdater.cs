using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolderTransformUpdater : MonoBehaviour
{
    public float transformX = 0.0f;
    public float transformY = 0.0f;
    public float transformZ = 0.0f;

    // rotationAngle is animated externally so it changes naturally.
    public float rotationAngle = 0.0f;

    [HideInInspector] public Transform itemHolderTransform;
    public Vector3 pos;

    [HideInInspector] public Transform playerTransform;

    void Awake()
    {
        // Automatically assign this object’s Transform to playerTransform
        playerTransform = transform;

        // Auto-assign itemHolderTransform if a child named "ItemHolder" exists
        Transform itemHolder = transform.Find("ItemHolder");
        if (itemHolder != null)
        {
            itemHolderTransform = itemHolder;
        }
        else
        {
            Debug.LogWarning("ItemHolder child not found under " + gameObject.name);
        }
    }

    void Update()
    {
        int s = HotbarSelector.slotSelected;
        List<Item> items = Inventory.instance.items;

        if (s < items.Count && itemHolderTransform != null)
        {
            Item i = items[s];
            pos = i.position;
            itemHolderTransform.localPosition = pos + new Vector3(transformX, transformY, transformZ);

            // Calculate the pivot point based on the item's scale and player scale
            Vector3 pivot = itemHolderTransform.position - new Vector3(
                i.scale.x * 1.5f / 10f * playerTransform.localScale.x,
                i.scale.y * 1.5f / 10f  * playerTransform.localScale.y,
                0f);

            // Rotate around the calculated pivot
            itemHolderTransform.RotateAround(pivot, Vector3.forward, rotationAngle * playerTransform.localScale.x);
            
        }
        else
        {
            pos = new Vector3(0.517f, 0.106f, 0.0f);
        }
    }
}
