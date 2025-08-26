using UnityEngine;

public class StartCatchEggs : MonoBehaviour
{
    public GameObject MainGame;
    public GameObject thumbnailmain;
    public GameObject thumbnailsprite;
    public GameObject tutorial;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        MainGame.SetActive(false);
        thumbnailmain.SetActive(true);
        thumbnailsprite.SetActive(true);
        tutorial.SetActive(false);
    }

    public void OpenTutorial()
    {
        MainGame.SetActive(false);
        thumbnailmain.SetActive(true);
        thumbnailsprite.SetActive(false);
        tutorial.SetActive(true);
    }
    public void OpenMainGame()
    {
        MainGame.SetActive(true);
        thumbnailmain.SetActive(false);
        thumbnailsprite.SetActive(false);
        tutorial.SetActive(false);
    }
}
