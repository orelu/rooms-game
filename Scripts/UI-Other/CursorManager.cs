using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D mainCursor;
    public Texture2D interactCursor;
    public Texture2D attackCursor;

    // Update is called once per frame
    void Update()
    {
        bool interacting = ItemLogic.instance.interactingObject!=null;
        bool giveAttackBonus = ItemLogic.instance.giveAttackBonus;
        if (giveAttackBonus) {
            Cursor.SetCursor(attackCursor, new Vector2(0, 0), CursorMode.Auto);
        } else if (interacting) {
            Cursor.SetCursor(interactCursor, new Vector2(0, 0), CursorMode.Auto);
        } else {
            Cursor.SetCursor(mainCursor, new Vector2(0, 0), CursorMode.Auto);
        }
        
    }
}
