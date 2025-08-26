using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class GeminiAPI : MonoBehaviour
{
    public static string apiUrl = "https://script.google.com/macros/s/AKfycby1VI1vUYm-PHe1SErSilmIbGIkRz86oCs-ir5FXlkVKdUwsiG-7bIoVn4-w66seLc/exec"; // your URL here

    public static void RequestQuestion(string topic, string difficulty, int grade, Action<string> callback)
    {
        Instance.StartCoroutine(SendRequest(topic, difficulty, grade, callback));
    }

    private static GeminiAPI _instance;
    public static GeminiAPI Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("GeminiAPI");
                _instance = obj.AddComponent<GeminiAPI>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    private static IEnumerator SendRequest(string topic, string difficulty, int grade, Action<string> callback)
    {
        var json = JsonUtility.ToJson(new
        {
            topic = topic,
            difficulty = difficulty,
            grade = grade
        });

        using (UnityWebRequest req = UnityWebRequest.PostWwwForm(apiUrl, ""))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var result = JsonUtility.FromJson<GeminiResponse>(req.downloadHandler.text);
                callback?.Invoke(result.generated);
            }
            else
            {
                Debug.LogError("Gemini API Error: " + req.error);
            }
        }
    }

    [Serializable]
    public class GeminiResponse
    {
        public string generated;
    }
}
