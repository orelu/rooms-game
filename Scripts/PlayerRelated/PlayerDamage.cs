using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{

    public CharacterStats c;

    

    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.O))
       {
        c.health -= 34;
        c.energy -= 7;
       } 
    }
}
