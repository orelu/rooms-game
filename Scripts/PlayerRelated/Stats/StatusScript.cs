using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusScript : MonoBehaviour
{

    [HideInInspector] public GameObject text;
    public int startValue;

    TextMeshProUGUI textMeshProText;

    void Start()
    {
        textMeshProText = text.GetComponent<TMPro.TextMeshProUGUI>();
        setValue(startValue);
    }

    void Awake()
    {
        if (text == null)
        {
            Transform textTransform = transform.Find("Text (TMP)");
            if (textTransform != null)
                text = textTransform.gameObject;
            else if (transform.childCount > 0)
                text = transform.GetChild(0).gameObject;
        }
    }


    public void setValue(int value)
    {
        textMeshProText = text.GetComponent<TMPro.TextMeshProUGUI>();
        textMeshProText.text = value.ToString();
    }
}
