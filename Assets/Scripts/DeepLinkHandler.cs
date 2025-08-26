using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeepLinkManager : MonoBehaviour
{
    public static DeepLinkManager Instance;

    public static List<string> CuratedGameIDs { get; private set; } = new List<string>();
    public static string CurrentStudentID { get; private set; } = "";
    public static List<string> CuratedLearningGaps { get; private set; } = new List<string>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Application.deepLinkActivated += OnDeepLinkActivated;

            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                OnDeepLinkActivated(Application.absoluteURL);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDeepLinkActivated(string url)
    {
        Debug.Log("🔗 Deep link received: " + url);

        Uri uri = new Uri(url);
        string host = uri.Host; // e.g., "inclass" or "athome"
        Dictionary<string, string> queryParams = ParseQueryString(uri.Query);

        CuratedGameIDs.Clear();
        CurrentStudentID = "";

        if (queryParams.TryGetValue("game_ids", out string gameIdString))
        {
            CuratedGameIDs = new List<string>(gameIdString.Split(','));
            Debug.Log("🎯 Parsed Game IDs: " + string.Join(", ", CuratedGameIDs));
        }

        if (queryParams.TryGetValue("learning_gaps", out string gaps))
        {
            CuratedLearningGaps = new List<string>(gaps.Split(','));
            Debug.Log("📌 Parsed Learning Gaps: " + string.Join(", ", CuratedLearningGaps));
        }

        if (host == "inclass")
        {
            Debug.Log("🧑‍🏫 Switching to InClassScene...");
            SceneManager.LoadScene("InClassScene");
        }
        else if (host == "athome")
        {
            if (queryParams.TryGetValue("student_id", out string studentId))
            {
                CurrentStudentID = studentId;
                Debug.Log("🏠 Parsed Student ID: " + CurrentStudentID);
            }

            Debug.Log("🎮 Switching to AtHomeScene...");
            SceneManager.LoadScene("AtHome");
        }
        else
        {
            Debug.LogWarning("❓ Unknown deep link host: " + host);
        }
    }

    Dictionary<string, string> ParseQueryString(string query)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        if (query.StartsWith("?")) query = query.Substring(1);
        string[] pairs = query.Split('&');
        foreach (string pair in pairs)
        {
            var kv = pair.Split('=');
            if (kv.Length == 2)
                result[Uri.UnescapeDataString(kv[0])] = Uri.UnescapeDataString(kv[1]);
        }
        return result;
    }
}
