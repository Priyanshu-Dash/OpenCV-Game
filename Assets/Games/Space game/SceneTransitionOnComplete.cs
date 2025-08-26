using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionOnComplete : MonoBehaviour
{
    public float delayBeforeLoad = 1.5f;
    public string targetScene = "InClassScene";
    public string allowedSourceScene = "InClassGames"; // ✅ Scene name where auto transition is allowed

    private void OnEnable()
    {
        QuestionManager.OnAllQuestionsCompleted += HandleCompletion;
    }

    private void OnDisable()
    {
        QuestionManager.OnAllQuestionsCompleted -= HandleCompletion;
    }

    private void HandleCompletion()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == allowedSourceScene)
        {
            Debug.Log($"📦 Transitioning to '{targetScene}' from '{currentScene}'...");
            Invoke(nameof(LoadScene), delayBeforeLoad);
        }
        else
        {
            Debug.Log($"⚠️ Scene transition blocked — current scene is '{currentScene}', not '{allowedSourceScene}'.");
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(targetScene);
    }
}
