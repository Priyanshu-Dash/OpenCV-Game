using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

[Serializable]
public class Part
{
    public string text;
}

[Serializable]
public class Content
{
    public Part[] parts;
}

[Serializable]
public class GeminiRequest
{
    public Content[] contents;
}

public class GeminiAPIS : MonoBehaviour
{
    [TextArea(2, 5)]
    public string apiKey = "YOUR_API_KEY_HERE"; // Set in Inspector

    private string Endpoint => $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=AIzaSyBMxpzLGOCxN9EoZsgfLhwJhDYovhj_XvE";

    public void GetQuestionFromGemini(string prompt, Action<string> onResponse)
    {
        StartCoroutine(SendPrompt(prompt, onResponse));
    }

    private IEnumerator SendPrompt(string prompt, Action<string> onResponse)
    {
        // Build the request object
        GeminiRequest request = new GeminiRequest
        {
            contents = new Content[]
            {
                new Content
                {
                    parts = new Part[]
                    {
                        new Part { text = prompt }
                    }
                }
            }
        };

        // Convert to JSON
        string json = JsonUtility.ToJson(request);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest requestWeb = new UnityWebRequest(Endpoint, "POST"))
        {
            requestWeb.uploadHandler = new UploadHandlerRaw(jsonBytes);
            requestWeb.downloadHandler = new DownloadHandlerBuffer();
            requestWeb.SetRequestHeader("Content-Type", "application/json");

            yield return requestWeb.SendWebRequest();

            if (requestWeb.result == UnityWebRequest.Result.Success)
            {
                string rawResponse = requestWeb.downloadHandler.text;
                string answer = ExtractQuestionAndAnswers(rawResponse);
                onResponse(answer);
            }
            else
            {
                Debug.LogError($"Error: {requestWeb.responseCode} - {requestWeb.error}\n{requestWeb.downloadHandler.text}");
                onResponse(null);
            }
        }
    }

    // Extract the question and multiple-choice answers from the response
    private string ExtractQuestionAndAnswers(string response)
    {
        try
        {
            int textIndex = response.IndexOf("\"text\":");
            if (textIndex >= 0)
            {
                int start = response.IndexOf("\"", textIndex + 7) + 1;
                int end = response.IndexOf("\"", start);
                string modelText = response.Substring(start, end - start).Trim();

                // You can adjust how you parse it based on expected format:
                // Example: "What is 4 + 5? A. 8 B. 9 C. 10"
                return modelText;
            }
            return "Error: Couldn't parse response.";
        }
        catch (Exception e)
        {
            return $"Error parsing response: {e.Message}";
        }
    }
}
