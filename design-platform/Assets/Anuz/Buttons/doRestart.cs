using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class doRestart : MonoBehaviour
{
    private Scene scene;

    private void Start()
    {
        scene = SceneManager.GetActiveScene();
    }
    public void dorestart()
    {
        Debug.Log("RESTART");
        Application.LoadLevel(scene.name);
    }
}
