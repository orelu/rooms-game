using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLogic : MonoBehaviour
{
    public static MouseLogic instance;

    [HideInInspector] public GameObject light;
    [HideInInspector] public Animator playerAnimator;

    public int radius = 5;
    Vector3 position;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of MouseLogic found.");
            return;
        }
        else
        {
            instance = this;
        }

        // Auto-assign MouseLight
        Transform lightTransform = transform.Find("MouseLight");
        if (lightTransform != null)
        {
            light = lightTransform.gameObject;
        }
        else
        {
            light = GameObject.Find("MouseLight");
            if (light == null)
                Debug.LogWarning("MouseLight GameObject not found.");
        }

        // Auto-assign Player's Animator
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            playerAnimator = playerObj.GetComponent<Animator>();
            if (playerAnimator == null)
                Debug.LogWarning("Animator not found on Player GameObject.");
        }
        else
        {
            Debug.LogWarning("Player GameObject not found.");
        }
    }

    void Update()
    {
        position = Input.mousePosition;
        position = Camera.main.ScreenToWorldPoint(new Vector3(position.x, position.y, Camera.main.nearClipPlane));

        transform.position = position;

        if (light != null)
        {
            light.transform.position = position;
        }
    }

    void LateUpdate()
    {
        int slotSelected = HotbarSelector.slotSelected;
        List<Item> items = Inventory.instance.items;
        bool wasRemoved = false;

        if (slotSelected <= items.Count - 1)
        {
            Item i = items[slotSelected];
            if (Input.GetMouseButtonDown(0))
            {
                wasRemoved = ItemLogic.instance.Use(0, i);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                wasRemoved = ItemLogic.instance.Use(1, i);
            }

            if (wasRemoved)
            {
                Inventory.instance.Remove(i);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                wasRemoved = ItemLogic.instance.Use(0, null);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                wasRemoved = ItemLogic.instance.Use(1, null);
            }
        }

        if (ItemLogic.instance.interactingObject == null)
        {
            enableLight();
        }
        else
        {
            disableLight();
        }
    }

    public void disableLight()
    {
        if (light != null)
            light.SetActive(false);
    }

    public void enableLight()
    {
        if (light != null)
            light.SetActive(true);
    }

    public bool isLightEnabled()
    {
        return light != null && light.activeSelf;
    }
}
