using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockToObject : MonoBehaviour
{

    public GameObject obj;
    public GameObject follower;

    
    void Update()
    {
        follower.transform.position = obj.transform.position;
    }
}
