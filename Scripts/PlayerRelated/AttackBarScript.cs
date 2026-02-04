using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBarScript : MonoBehaviour
{

    public BarScript BarScript;
    public GameObject Bar;

    

    // Update is called once per frame
    void Update()
    {
        float attackCooldown=ItemLogic.instance.attackCooldown;
        float maxAttackCooldown=ItemLogic.instance.maxAttackCooldown;
        if (attackCooldown>0) {
            if (!Bar.activeSelf) {
                Bar.SetActive(true);
            }
            BarScript.SetMinValue(-maxAttackCooldown);
            BarScript.SetMaxValue(0f);
            BarScript.setValue(-attackCooldown);
        } else {
            Bar.SetActive(false);
        }
    }
}
