using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public string selectedMode; // Current learning mode
    public int score = 0;
    int highScore;
    public TMP_Text scoretext , mistakestext;
    public TMP_Text yourscore;
    public TMP_Text highscore;
    public int maxMistakes = 3;
    private int mistakesleft = 0;
    public GameObject gameoverpanel;
    public GameObject spawner, spawner2, spawner3, spawner4, spawner5, spawner6;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Get the selected mode from PlayerPrefs
        selectedMode = PlayerPrefs.GetString("SelectedMode", "EvenNumbers"); // Default mode
        gameoverpanel.SetActive(false);
    }

    void Start()
    {
        if (selectedMode == "AS" || selectedMode == "MD")
        {
            Destroy(spawner); Destroy(spawner3); Destroy(spawner4); Destroy(spawner5); Destroy(spawner6);
            maxMistakes =2;
        }
        else if (selectedMode == "OddNumbers" || selectedMode == "EvenNumbers" || selectedMode == "PrimeNumbers")
        {
            Destroy(spawner2); Destroy(spawner3); Destroy(spawner4); Destroy(spawner5); Destroy(spawner6);
            maxMistakes =1;
        }
        else if (selectedMode == "SentenceFormation")
        {
            Destroy(spawner); Destroy(spawner2); Destroy(spawner3); Destroy(spawner4); Destroy(spawner5);
            maxMistakes =3;
        }
        else if (selectedMode == "VerbsAndTenses")
        {
            Destroy(spawner); Destroy(spawner2); Destroy(spawner4); Destroy(spawner5); Destroy(spawner6);
            maxMistakes =2;
        }
        else if (selectedMode == "Vowels")
        {
            Destroy(spawner); Destroy(spawner2); Destroy(spawner3); Destroy(spawner5); Destroy(spawner6);
            maxMistakes =1;
        }
        else if (selectedMode == "PAC")
        {
            Destroy(spawner); Destroy(spawner2); Destroy(spawner3); Destroy(spawner4); Destroy(spawner6);
            maxMistakes =3;
        }
        mistakesleft = maxMistakes;
        scoretext.text = "Score: 0";
        mistakestext.text = "Mistakes Left: "+maxMistakes.ToString();
        highScore = PlayerPrefs.GetInt(selectedMode.ToString()+"HighScore", 0);
    }

    public void CheckNumber(bool valid)
    {
        bool isCorrect = valid;

        if (isCorrect)
        {
            score += 5;
            scoretext.text = "Score: " + score.ToString();
        }
        else
        {
            mistakesleft--;
            mistakestext.text = "Mistakes Left: "+mistakesleft.ToString();
            if (mistakesleft <=0)
            {
                GameOver();
            }
        }
    }

    public void GameOver()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(selectedMode.ToString()+"HighScore", highScore); // Save the new high score
            PlayerPrefs.Save(); // Ensure data is written to storage
        }
        Time.timeScale = 0f;
        gameoverpanel.SetActive(true);
        yourscore.text = "YourScore: " + score.ToString();
        highscore.text = "HighScore: " + highScore.ToString();
    }

    public void Back(){
        SceneManager.LoadScene(0);
    }
}
