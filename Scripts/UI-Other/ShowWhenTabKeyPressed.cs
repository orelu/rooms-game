using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowWhenTabKeyPressed : MonoBehaviour
{
    public GameObject speedBar;
    public GameObject strengthBar;
    public GameObject defenceBar;
    public GameObject goldBar;
    public GameObject equipmentHotbar;

    void Start()
    {
        activate(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            activate(true);
        } 

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            activate(false);
        }
    }


    void activate(bool isActivated)
    {
        speedBar.SetActive(isActivated);
        strengthBar.SetActive(isActivated);
        defenceBar.SetActive(isActivated);
        goldBar.SetActive(isActivated);
        //equipmentHotbar.SetActive(isActivated);
    }
}
