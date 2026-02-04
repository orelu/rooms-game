using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDown : MonoBehaviour
{

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localScale+= new Vector3(0, 10, 0);
    }
}
