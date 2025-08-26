using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public GameObject selecmode , intials , quit;
    // Start is called before the first frame update
    void Start()
    {
        selecmode.SetActive(false);
        quit.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Onclickplay(){
        selecmode.SetActive(true);
        intials.SetActive(false);
    }

    public void SelectMode(string mode)
    {
        PlayerPrefs.SetString("SelectedMode", mode); // Save the selected mode
        SceneManager.LoadScene(1); // Load the game scene
    }

    public void back(){
        selecmode.SetActive(false);
        intials.SetActive(true);
    }

    public void Quit(){
        quit.SetActive(true);
        Application.Quit();
    }
}
