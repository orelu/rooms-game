using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBarScript : MonoBehaviour
{
    private Slider slider;
    private EnemyBaseBehavior enemyStats;

    void Start()
    {

        enemyStats = transform.GetComponent<EnemyBaseBehavior>();
        

        if (enemyStats == null) 
        {
            enemyStats = transform.parent.GetComponent<EnemyBaseBehavior>();
        }

        if (enemyStats == null && transform.parent.parent!=null) 
        {
            enemyStats = transform.parent.parent.GetComponent<EnemyBaseBehavior>();
        }

        // Get the Slider component from child GameObjects
        slider = GetComponentInChildren<Slider>();

        if (enemyStats == null)
        {
            Debug.LogWarning("EnemyBaseBehavior component not found on " + gameObject.name);
        }

        if (slider == null)
        {
            Debug.LogWarning("Slider component not found in children of " + gameObject.name);
        }

        if (enemyStats != null && slider != null)
        {
            SetMaxValue(enemyStats.maxHealth);
            setValue(enemyStats.maxHealth);
        }
    }

    public void SetMaxValue(int value)
    {
        if (slider != null)
        {
            slider.maxValue = value;
        }
    }

    public void setValue(float value)
    {
        if (slider != null)
        {
            slider.value = value;
        }
    }

    void Update()
    {
        if (enemyStats != null && slider != null)
        {
            setValue(enemyStats.health);
        }
    }
}
