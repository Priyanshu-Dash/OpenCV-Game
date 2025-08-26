using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuestionManager : MonoBehaviour
{
    [Header("References")]
    public GameObject resultCanvas;
    public Transform vehicle;
    public GeminiUITest geminiUITest;
    public Text questionProgressText;

    [Header("Settings")]
    public int lives = 3;
    public float moveDistance = 5f;
    public int totalQuestions = 10;

    private int currentQuestion = 0;
    private string currentScene;

    public static event System.Action OnAllQuestionsCompleted;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;

        // ✅ Show progress text only in InClassGames scene
        if (questionProgressText != null)
        {
            questionProgressText.gameObject.SetActive(currentScene == "InClassGames");
        }
    }

    public void HandleAnswerAndNextQuestion(bool isCorrect)
    {
        resultCanvas.transform.position += vehicle.forward * moveDistance;

        if (!isCorrect)
        {
            lives--;
            Debug.Log("❌ Incorrect! Lives left: " + lives);

            if (lives <= 0)
            {
                Debug.Log("💀 Lives exhausted! Continuing until all questions are answered...");
            }
        }
        else
        {
            Debug.Log("✅ Correct Answer!");
        }

        currentQuestion++;

        // ✅ Update progress text only in InClassGames scene
        if (questionProgressText != null && currentScene == "InClassGames")
        {
            questionProgressText.text = $"{currentQuestion}/{totalQuestions}";
        }

        if (currentQuestion >= totalQuestions)
        {
            Debug.Log("🎉 All questions answered.");
            OnAllQuestionsCompleted?.Invoke();
            return;
        }

        geminiUITest.OnSendPrompt();
    }
}
