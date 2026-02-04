using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterStats : MonoBehaviour
{
   public int maxHealth = 100;
   public int maxEnergy = 100;
   
   public int health = 100;
   public Stat healthRegeneration;
   public int energy = 100;
   public Stat energyRegeneration;
   public Stat speed;
   public int money = 0;
   public Stat armor;
   public Stat damage;

   public BarScript healthBar;
   public BarScript energyBar;

   public StatusScript speedText;
   public StatusScript moneyText;
   public StatusScript damageText;
   public StatusScript armorText;

   private int regenCounter = 250;

   void Awake ()
   {
    health = maxHealth;
   }

   void Update()
   {
        healthBar.setValue(health);
        energyBar.setValue(energy);
        speedText.setValue(speed.getValue());
        armorText.setValue(armor.getValue());
        damageText.setValue(damage.getValue());
        moneyText.setValue(money);

        healthBar.SetMaxValue(maxHealth);
        energyBar.SetMaxValue(maxEnergy);
    
   }

   void FixedUpdate()
   {
    if (regenCounter <= 0)
    {
        health += healthRegeneration.getValue();
        energy += energyRegeneration.getValue();
        regenCounter = 250;

        if (health > maxHealth) {health = maxHealth;}
        if (energy > maxEnergy) {energy = maxEnergy;}
    } else {
        regenCounter -= 1;
    }
   }

   public void TakeDamage(int damage)
   {

    damage -= armor.getValue();
    damage = Mathf.Clamp(damage, 0, int.MaxValue);

    health -= damage;

    if (health<= 0)
    {
        Die();
    }
   }

   public void UseEnergy(int amountUsed)
   {

    if (amountUsed >= energy)
    {
        energy -= amountUsed;
    }
   }
    

    public virtual void Die()
    {
        Debug.Log(transform.name +" died.");
    }

}

