using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarterScene : MonoBehaviour
{

    public Transform transform;
    public GameObject dontDestroy;

    // Update is called once per frame
    void Update()
    {

        if (transform.position.x > 20) {
            // Call this from any other script to transition scenes
            SceneManager.LoadSceneAsync("CombatLevel");
            SceneManager.UnloadSceneAsync("StarterLevel");
            transform.position+= new Vector3(-20, 0, 0);

        }
        
    }

    void Awake() {
        DontDestroyOnLoad(dontDestroy);
    }
}
