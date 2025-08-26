using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using UnityEngine.Android;

public class SpeechRecognizer : MonoBehaviour
{
    [Header("Dependencies")]
    public AnimalQuiz animalQuiz;
    [Tooltip("Paste your Google STT API key here.")]
    public string apiKey;

    [Header("Recording Settings")]
    public int recordingDuration = 5; // seconds
    public Button recordButton;

    private AudioClip recordedClip;
    private const int sampleRate = 16000;

    void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        if (recordButton != null)
            recordButton.onClick.AddListener(StartRecording);
    }

    public void StartRecording()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogWarning("No microphone detected.");
            return;
        }

        recordedClip = Microphone.Start(null, false, recordingDuration, sampleRate);
        Debug.Log("Recording started...");
        StartCoroutine(StopRecordingAfterDelay(recordingDuration));
    }

    private IEnumerator StopRecordingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Microphone.End(null);
        Debug.Log("Recording stopped.");

        byte[] audioData = WavUtility.FromAudioClip(recordedClip);
        StartCoroutine(SendToGoogleAPI(audioData));
    }

    private IEnumerator SendToGoogleAPI(byte[] audioBytes)
    {
        if (audioBytes == null || audioBytes.Length == 0)
        {
            Debug.LogError("Audio byte array is empty or null.");
            yield break;
        }

        string base64Audio = System.Convert.ToBase64String(audioBytes);
        Debug.Log("Base64 Audio Length: " + base64Audio.Length);

        string jsonRequest = $"{{" +
            "\"config\": {{" +
                "\"encoding\":\"LINEAR16\"," +
                "\"sampleRateHertz\":16000," +
                "\"languageCode\":\"en-US\"" +
            "}}," +
            $"\"audio\": {{\"content\":\"{base64Audio}\"}}" +
        "}}";

        Debug.Log("Prepared JSON Request:\n" + jsonRequest.Substring(0, Mathf.Min(jsonRequest.Length, 1000)) + (jsonRequest.Length > 1000 ? "...(truncated)" : ""));

        string apiUrl = $"https://speech.googleapis.com/v1/speech:recognize?key={apiKey}";
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonRequest);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log("Sending request to: " + apiUrl);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Request Error: " + request.error);
            Debug.LogError("Response Code: " + request.responseCode);
            Debug.LogError("Response Text: " + request.downloadHandler.text);
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("Success Response: " + responseText);

            string transcript = ParseTranscript(responseText);
            if (!string.IsNullOrEmpty(transcript))
            {
                Debug.Log("Recognized Transcript: " + transcript);
                animalQuiz.OnSpeechRecognized(transcript);
            }
            else
            {
                Debug.Log("No transcript recognized.");
            }
        }
    }

    private string ParseTranscript(string json)
    {
        try
        {
            var wrapper = JsonUtility.FromJson<GoogleResponseWrapper>(json);
            if (wrapper.results.Length > 0 && wrapper.results[0].alternatives.Length > 0)
                return wrapper.results[0].alternatives[0].transcript;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to parse transcript: " + e.Message);
        }
        return "";
    }

    [System.Serializable]
    private class GoogleResponseWrapper
    {
        public Result[] results;
    }

    [System.Serializable]
    private class Result
    {
        public Alternative[] alternatives;
    }

    [System.Serializable]
    private class Alternative
    {
        public string transcript;
        public float confidence;
    }
}
