using UnityEngine;

public class SolarQuizA : MonoBehaviour
{
    public GameObject StartPanel;
    public GameObject Game;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPanel.SetActive(true);
        Game.SetActive(false);
    }

    public void StartGame()
    {
        StartPanel.SetActive(false);
        Game.SetActive(true);
    }
}
