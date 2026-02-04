using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public int radius = 5;
    [HideInInspector] public GameObject player;
    public GameObject dependantObject;
    public Item item;

    void Awake()
    {
        // Auto-find player (by tag first, then by name)
        player = GameObject.FindWithTag("Player");
        if (player == null)
            player = GameObject.Find("Player");
    }

    void OnMouseOver()
    {
        if (player != null && Input.GetMouseButtonDown(1) && Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            if (dependantObject != null)
            {
                dependantObject.SetActive(false);
            }
            else
            {
                Debug.LogError("DependantObject is null!");
            }
            PickUp();
        }
    }

    void PickUp()
    {
        if (item == null)
        {
            Debug.LogError("Item is null!");
            return;
        }

        if (Inventory.instance == null)
        {
            Debug.LogError("Inventory.instance is null!");
            return;
        }

        bool wasSuccesful = Inventory.instance.Add(item);

        if (wasSuccesful)
        {
            Destroy(gameObject);
            ItemLogic.instance.interactingObject = null;
        }
    }
}
