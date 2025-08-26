using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerABR : MonoBehaviour
{
    public GameObject Thumbnail;
    public GameObject MainGame;
    public GameObject Tutorial;
    public GameObject GameCompletedPanel;

    public TextMeshProUGUI countdownText;

    private float timeRemaining = 60f;
    private bool isGameRunning = false;

    void Start()
    {
        Thumbnail.SetActive(true);
        MainGame.SetActive(false);
        Tutorial.SetActive(false);
        GameCompletedPanel.SetActive(false);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isGameRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                int seconds = Mathf.CeilToInt(timeRemaining);
                countdownText.text = "00:" + seconds.ToString("D2");
            }
            else
            {
                isGameRunning = false;
                MainGame.SetActive(false);
                countdownText.gameObject.SetActive(false);
                GameCompletedPanel.SetActive(true);
                Invoke("LoadAtHomeScene", 2f);
            }
        }
    }

    public void StartGame()
    {
        Thumbnail.SetActive(false);
        MainGame.SetActive(true);
        Tutorial.SetActive(false);

        // Start countdown
        timeRemaining = 60f;
        isGameRunning = true;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = "01:00";
        }
    }

    public void OpenTutorial()
    {
        Thumbnail.SetActive(false);
        MainGame.SetActive(false);
        Tutorial.SetActive(true);
    }

    void LoadAtHomeScene()
    {
        SceneManager.LoadScene("AtHome");
    }
}
