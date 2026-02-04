using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultDoorLogic : MonoBehaviour
{
    public GameObject mouse;
    public GameObject sprite1;
    public GameObject sprite2;

    public Sprite icon1;
    public Sprite icon2;

    public Animator playerAnimator;

    public void ClickLogic() {
            
        sprite1.GetComponent<TwoSpriteLogic>().isActive = false;
        sprite2.GetComponent<TwoSpriteLogic>().isActive = false;

        sprite1.GetComponent<BoxCollider2D>().enabled = false;
        sprite2.GetComponent<BoxCollider2D>().enabled = false;

        sprite1.GetComponent<SpriteRenderer>().sprite = icon1;
        sprite2.GetComponent<SpriteRenderer>().sprite = icon2;
            

        

    }

    void OnMouseEnter() {
        ItemLogic.instance.interactingObject = gameObject;
    }

    void OnMouseExit() {
        ItemLogic.instance.interactingObject = null;
    }
}
