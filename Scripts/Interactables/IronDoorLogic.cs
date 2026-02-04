using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronDoorLogic : MonoBehaviour
{
    private GameObject sprite1;
    private GameObject sprite2;
    private Animator playerAnimator;
    private CameraFollow cameraFollow;

    public Sprite icon1;
    public Sprite icon2;

    void Awake()
    {
        // Self
        sprite1 = gameObject;

        // Find "Iron Door Bottom" in scene
        sprite2 = GameObject.Find("Iron Door Bottom");

        // Get Animator from Player
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            playerAnimator = player.GetComponent<Animator>();

        // Get CameraFollow from Main Camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
            cameraFollow = mainCam.GetComponent<CameraFollow>();
    }

    public void ClickLogic()
    {
        if (sprite1 != null)
        {
            sprite1.GetComponent<TwoSpriteLogic>().isActive = false;
            sprite1.GetComponent<BoxCollider2D>().enabled = false;
            sprite1.GetComponent<SpriteRenderer>().sprite = icon1;
        }

        if (sprite2 != null)
        {
            sprite2.GetComponent<TwoSpriteLogic>().isActive = false;
            sprite2.GetComponent<BoxCollider2D>().enabled = false;
            sprite2.GetComponent<SpriteRenderer>().sprite = icon2;
        }

        if (cameraFollow != null)
            cameraFollow.shake = 1.0f;
    }

    void OnMouseEnter()
    {
        ItemLogic.instance.interactingObject = gameObject;
    }

    void OnMouseExit()
    {
        ItemLogic.instance.interactingObject = null;
    }
}
