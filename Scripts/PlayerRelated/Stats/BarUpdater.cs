using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarUpdater : MonoBehaviour
{
    [HideInInspector] public BarScript healthBar;
    [HideInInspector] public BarScript energyBar;
    [HideInInspector] public ExperienceBarUpdater experienceBar;

    [HideInInspector] public StatusScript speedText;
    [HideInInspector] public StatusScript moneyText;
    [HideInInspector] public StatusScript damageText;
    [HideInInspector] public StatusScript armorText;

    public PlayerStats stats;

    void Awake()
    {
        // Find the Canvas in the scene
        GameObject canvas = GameObject.Find("MainCanvas");
        if (canvas == null)
        {
            Debug.LogWarning("Canvas not found in the scene.");
            return;
        }

        // Find the "Bars" child under Canvas
        Transform bars = canvas.transform.Find("Bars");
        if (bars == null)
        {
            Debug.LogWarning("Bars object not found as a child of Canvas.");
            return;
        }

        // Auto-assign components by name
        healthBar = FindInChildrenByName<BarScript>(bars, "Health Bar");
        energyBar = FindInChildrenByName<BarScript>(bars, "Energy Bar");
        experienceBar = FindInChildrenByName<ExperienceBarUpdater>(bars, "ExperienceBar");

        speedText = FindInChildrenByName<StatusScript>(bars, "Speed Bar");
        moneyText = FindInChildrenByName<StatusScript>(bars, "Gold Bar");
        damageText = FindInChildrenByName<StatusScript>(bars, "Strength Bar");
        armorText = FindInChildrenByName<StatusScript>(bars, "Defence Bar");
    }

    void Update()
    {
        if (stats == null) return;

        healthBar.setValue(stats.health);
        experienceBar.setValue(stats.experience_this_level, stats.level);
        energyBar.setValue(stats.energy);

        speedText.setValue(stats.speed.getValue());
        armorText.setValue(stats.armor.getValue());
        damageText.setValue(stats.damage.getValue());
        moneyText.setValue(stats.money);

        healthBar.SetMaxValue(stats.maxHealth);
        energyBar.SetMaxValue(stats.maxEnergy);
        experienceBar.SetMaxValue(stats.checkExperienceToNextLevel(stats.level));
    }

    // Helper method to find a child by name and get a specific component
    private T FindInChildrenByName<T>(Transform parent, string name) where T : Component
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == name)
                return child.GetComponent<T>();
        }

        Debug.LogWarning($"Component {typeof(T).Name} not found on child '{name}' under '{parent.name}'");
        return null;
    }
}
