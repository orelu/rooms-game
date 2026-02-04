using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarScript : MonoBehaviour
{
    [HideInInspector] public Slider slider;
    [HideInInspector] public GameObject text;

    public int startValue;

    private TextMeshProUGUI textMeshProText;

    void Awake()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        
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
        setValue(startValue);
    }

    public void SetMaxValue(float value)
    {
        slider.maxValue = value;
    }

    public void SetMinValue(float value)
    {
        slider.minValue = value;
    }

    public void setValue(float value)
    {
        slider.value = value;

        if (textMeshProText != null)
            textMeshProText.text = $"{value}/{slider.maxValue}";
    }
}
