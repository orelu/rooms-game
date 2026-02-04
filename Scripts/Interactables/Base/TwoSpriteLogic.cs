using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoSpriteLogic : MonoBehaviour
{
    public GameObject spriteOne;
    public GameObject spriteTwo;

    public Sprite tileUnselectedOne;
    public Sprite tileSelectedOne;

    public Sprite tileUnselectedTwo;
    public Sprite tileSelectedTwo;

    public int radius;

    BoxCollider2D colliderOne;
    BoxCollider2D colliderTwo;
    SpriteRenderer spriteRendererOne;
    SpriteRenderer spriteRendererTwo;

    public bool isActive = true;
    private GameObject player;

    void Awake()
    {
        // Automatically find player by tag
        player = GameObject.FindGameObjectWithTag("Player");

        colliderOne = spriteOne.GetComponent<BoxCollider2D>();
        spriteRendererOne = spriteOne.GetComponent<SpriteRenderer>();
        spriteRendererOne.sprite = tileUnselectedOne;

        colliderTwo = spriteTwo.GetComponent<BoxCollider2D>();
        spriteRendererTwo = spriteTwo.GetComponent<SpriteRenderer>();
        spriteRendererTwo.sprite = tileUnselectedTwo;
    }

    void OnMouseEnter()
    {
        if (player != null && Vector3.Distance(spriteOne.transform.position, player.transform.position) < radius)
        {
            spriteRendererOne.sprite = tileSelectedOne;
            spriteRendererTwo.sprite = tileSelectedTwo;
        }
    }

    void OnMouseExit()
    {
        if (isActive)
        {
            spriteRendererOne.sprite = tileUnselectedOne;
            spriteRendererTwo.sprite = tileUnselectedTwo;
        }
    }
}
