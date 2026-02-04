using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class PlayerStatsData
{
    public int maxHealth;
    public int maxEnergy;
    public int health;
    public int energy;
    public int money;

    public int healthRegeneration;
    public int energyRegeneration;
    public int speed;
    public int armor;
    public int damage;
    public int experience;
    public int experience_this_level;
    public int level;
}

public class PlayerStats : MonoBehaviour
{

    public static PlayerStats instance;

    void Awake()
    {

        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerStats found.");
            return;
        } else {
            instance = this;

        }


        
    }

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
   private int regenCounter = 250;

   public int experience = 0;
   public int level = 0;
   public int experience_this_level = 0;

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

   public int checkExperience(int l) {
        if (l > 0 && l < 16)
        {
            return l * l + 5 * l;
        }
        else if (l >= 16)
        {
            return 37 * l - 256;
        }
        else
        {
            return 0; 
        }
   }

    public double checkLevel(int e) {
        if (e < 336)
        {
            return (-5 + Math.Sqrt(25 + 4 * e)) / 2;
        }
        else
        {
            return (e + 256) / 37;
        }
    }

    public int checkExperienceToNextLevel(int l) {
        return checkExperience(l+1)-checkExperience(l);
    }

   public void gainExperience(int experienceGained) 
   {
        experience+=experienceGained;
    
        level = (int) Math.Floor(checkLevel(experience));

        experience_this_level = experience-checkExperience(level);

        Debug.Log("Level:" + level.ToString()+". Experience: " + experience.ToString());

        
   }
    

    public virtual void Die()
    {
        Debug.Log(transform.name +" died.");
    }

    public PlayerStatsData ToData()
    {
        return new PlayerStatsData
        {
            maxHealth = this.maxHealth,
            maxEnergy = this.maxEnergy,
            health = this.health,
            energy = this.energy,
            money = this.money,
            healthRegeneration = healthRegeneration.getValue(),
            energyRegeneration = energyRegeneration.getValue(),
            speed = speed.getValue(),
            armor = armor.getValue(),
            damage = damage.getValue(),
            experience = this.experience,
            experience_this_level = this.experience_this_level,
            level = this.level
        };
    }

    public void FromData(PlayerStatsData data)
    {
        this.maxHealth = data.maxHealth;
        this.maxEnergy = data.maxEnergy;
        this.health = data.health;
        this.energy = data.energy;
        this.money = data.money;
        this.healthRegeneration.setValue(data.healthRegeneration);
        this.energyRegeneration.setValue(data.energyRegeneration);
        this.speed.setValue(data.speed);
        this.armor.setValue(data.armor);
        this.damage.setValue(data.damage);
        this.experience = data.experience;
        this.level = data.level;
        this.experience_this_level = data.experience_this_level;
    }

}