using UnityEngine;

public class SolarGameA : MonoBehaviour
{
    public GameObject StartPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPanel.SetActive(true);
    }

    public void StartGame()
    {
        StartPanel.SetActive(false);
    }
    
}
