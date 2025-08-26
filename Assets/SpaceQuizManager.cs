using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

namespace SpaceQuiz
{
    [System.Serializable]
    public class Question
    {
        public string question;
        public List<string> options;
        public string correct;
    }

    [System.Serializable]
    public class QuestionList
    {
        public List<Question> questions;
    }
}

public class SpaceQuizManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public TextMeshPro optionA;  // Tagged "A"
    public TextMeshPro optionB;  // Tagged "B"
    public TextMeshPro optionC;  // Tagged "C"

    private SpaceQuiz.QuestionList questionList;
    private SpaceQuiz.Question currentQuestion;

    [Header("Lives")]
    public GameObject[] heartIcons;     // Assign 3 hearts in order
    public int maxLives = 3;
    private int currentLives;

    [Header("Moving Object")]
    public Transform objectToMove;              // Assign in Inspector
    public float moveDistanceZ = 50f;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;       // Assign in Inspector
    public Transform spaceship;            // Assign spaceship GameObject
    public float shakeDuration = 1f;       // Shake time for incorrect answer
    public float shakeStrength = 0.5f;     // How strong the shake is

    public string QuestionJSON;

    void Start()
    {
        LoadQuestionsFromJson();
        ShowRandomQuestion();
        currentLives = maxLives;
        UpdateHeartIcons();
    }
    void MoveObjectForward()
    {
        if (objectToMove != null)
        {
            objectToMove.position += new Vector3(0, 0, moveDistanceZ);
        }
    }

    void LoadQuestionsFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(QuestionJSON);
        if (jsonFile != null)
        {
            questionList = JsonUtility.FromJson<SpaceQuiz.QuestionList>(jsonFile.text);
        }
        else
        {
            Debug.LogError("WordCatcher.json not found in Resources folder.");
        }
    }

    void ShowRandomQuestion()
    {
        if (questionList == null || questionList.questions.Count == 0) return;

        int index = Random.Range(0, questionList.questions.Count);
        currentQuestion = questionList.questions[index];

        questionText.text = currentQuestion.question;

        optionA.text = currentQuestion.options[0];
        optionB.text = currentQuestion.options[1];
        optionC.text = currentQuestion.options[2];

        MoveObjectForward();
    }

    void OnTriggerEnter(Collider other)
    {
        string selectedOption = "";

        switch (other.tag)
        {
            case "A":
                selectedOption = optionA.text;
                break;
            case "B":
                selectedOption = optionB.text;
                break;
            case "C":
                selectedOption = optionC.text;
                break;
            default:
                return; // Not a valid option tag
        }

        if (selectedOption == currentQuestion.correct)
        {
            Debug.Log("✅ Correct: " + selectedOption);
        }
        else
        {
            currentLives--;
            UpdateHeartIcons();
            Debug.Log("❌ Incorrect: " + selectedOption + " | Lives Left: " + currentLives);

            // Shake the spaceship
            if (spaceship != null)
            {
                spaceship.DOShakePosition(shakeDuration, new Vector3(shakeStrength, shakeStrength, 0), 10, 90f, false, true);
            }

            if (currentLives <= 0)
            {
                StartCoroutine(HandleGameOver());
                return;
            }
        }

        ShowRandomQuestion();
    }

    IEnumerator HandleGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "InclassGames")
        {
            SceneManager.LoadScene("InClassScene");
        }
        else
        {
            SceneManager.LoadScene(currentScene); // Restart current scene
        }
    }

    void UpdateHeartIcons()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < currentLives);
        }
    }
}
