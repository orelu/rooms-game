using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceBarUpdater : MonoBehaviour
{
    [HideInInspector] public Slider slider;
    [HideInInspector] public GameObject text;

    public int startValue;

    TextMeshProUGUI textMeshProText;

    void Awake()
    {
        // Auto-assign slider from self
        if (slider == null)
            slider = GetComponent<Slider>();

        // Auto-assign text from child named "Text" or first child
        if (text == null)
        {
            Transform textTransform = transform.Find("Text (TMP)");
            if (textTransform != null)
                text = textTransform.gameObject;
            else if (transform.childCount > 0)
                text = transform.GetChild(0).gameObject;
        }

        // Get the TMP component
        if (text != null)
            textMeshProText = text.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        setValue(startValue, 0);
    }


    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
    }

    public void SetMinValue(float value)
    {
        slider.minValue = value;
    }


    public void setValue(float value, int level)
    {
        slider.value = value;
        if (text!=null) {
            textMeshProText.text = "Level "+level.ToString();
        }
        
    }
}
