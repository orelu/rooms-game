using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchLogic : MonoBehaviour
{
    [HideInInspector] public GameObject spriteUnselected; 
    [HideInInspector] public GameObject spriteSelected;
    [HideInInspector] public GameObject player;

    public int radius;

    void Awake()
    {
        // Auto-assign references
        spriteSelected = gameObject;                // Self
        spriteUnselected = transform.parent.gameObject; // Parent

        // Find player by tag first, then by name
        player = GameObject.FindWithTag("Player");
        if (player == null)
            player = GameObject.Find("Player");
    }

    void OnMouseEnter()
    {
        if (player != null && Vector3.Distance(spriteUnselected.transform.position, player.transform.position) < radius)
        {
            spriteSelected.GetComponent<SpriteRenderer>().enabled = true;
            spriteUnselected.GetComponent<SpriteRenderer>().enabled = false;
            ItemLogic.instance.interactingObject = gameObject;
        }
    }

    void OnMouseExit()
    {
        spriteSelected.GetComponent<SpriteRenderer>().enabled = false;
        spriteUnselected.GetComponent<SpriteRenderer>().enabled = true;
        ItemLogic.instance.interactingObject = null;
    }
}
