using UnityEngine;

public class QuestionGameManager : MonoBehaviour
{
    private GameObject currentGame;

    void Start()
    {
        var student = StudentProfileManager.Instance.GetStudentProfile();
        var curated = FindObjectOfType<GameCurationManager>();
        var nextGame = curated.GetNextGame();

        if (nextGame == null)
        {
            Debug.LogWarning("🚫 No suitable game found.");
            return;
        }

        Debug.Log("✅ Curated gameId: " + nextGame.gameId);

        GameObject prefabToLoad = GamePrefabDatabase.Instance.GetPrefabByGameId(nextGame.gameId);

        if (prefabToLoad == null)
        {
            Debug.LogError("❌ No prefab found for gameId: " + nextGame.gameId);
            return;
        }

        Debug.Log("✅ Found prefab: " + prefabToLoad.name);

        string topic = nextGame.learningOutcomes[0];
        string difficulty = nextGame.difficulty;
        int grade = int.Parse(student.grade.Replace("A", "").Replace("B", "").Trim());

        GeminiAPI.RequestQuestion(topic, difficulty, grade, (response) =>
        {
            Debug.Log("📩 Received AI question:\n" + response);
            var aiQ = AIQuestionParser.FromGeminiText(response);

            currentGame = Instantiate(prefabToLoad);
            Debug.Log("🎮 Instantiated prefab: " + prefabToLoad.name);

            var ui = currentGame.GetComponent<AIQuestionUI>();
            if (ui != null)
            {
                Debug.Log("✅ Found AIQuestionUI script. Setting up...");
                ui.Setup(aiQ);
            }
            else
            {
                Debug.LogError("❌ AIQuestionUI script NOT found on prefab!");
            }
        });
    }

}
