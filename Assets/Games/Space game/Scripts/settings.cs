using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class settings : MonoBehaviour
{
    public GameObject maingame;
    public GameObject cameraTest;

    void Start()
    {
        maingame.SetActive(false);
    }

    public void start(){
        maingame.SetActive(true);
        cameraTest.SetActive(false);
    }
}
