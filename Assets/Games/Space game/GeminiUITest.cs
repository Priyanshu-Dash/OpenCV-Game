using UnityEngine;
using TMPro; // Importing TextMeshPro namespace
using UnityEngine.UI; // Importing Unity UI namespace for InputField and Text

public class GeminiUITest : MonoBehaviour
{
    public InputField promptField;        // InputField for entering the prompt
    public Text resultText;               // Text component for displaying the result (e.g., question and answers)
    public GeminiAPIS geminiAPI;
    public QuestionManager questionManager;

    // TextMeshPro components for options A, B, C (for world-space text)
    public TextMeshPro optionA;
    public TextMeshPro optionB;
    public TextMeshPro optionC;

    private string correctAnswer; // Variable to store the correct answer (A, B, or C)
    public string prompt;

    void Start()
    {
        OnSendPrompt();
    }

    public void OnSendPrompt()
    {
        string currentprompt = prompt;
        geminiAPI.GetQuestionFromGemini(prompt, (response) =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                resultText.text = response;  // Display the full question and answers
                ParseAndDisplayOptions(response); // Parse and display options in world space
            }
            else
            {
                resultText.text = "Failed to get response.";
            }
        });
    }

    // Function to parse the response and set options in world space
    private void ParseAndDisplayOptions(string response)
    {
        // Example response: "Which of the following options is NOT an adverb A. Greedy B. Rarely (Correct) C. Creatively"

        // Split parts
        string[] parts = response.Split(new string[] { " A.", " B.", " C." }, System.StringSplitOptions.None);

        if (parts.Length < 4)
        {
            Debug.LogError("❌ Response parsing failed. Expected at least 4 parts.");
            return;
        }

        string question = parts[0].Trim();           // "Which of the following options is NOT an adverb?"
        string optionAAnswer = parts[1].Trim();       // "Greedy" or "Greedy (Correct)"
        string optionBAnswer = parts[2].Trim();       // "Rarely" or "Rarely (Correct)"
        string optionCAnswer = parts[3].Trim();       // "Creatively" or "Creatively (Correct)"

        // Set world-space option texts
        optionA.text = "A";
        optionB.text = "B";
        optionC.text = "C";

        // ✅ Detect which option contains (Correct) and set correctAnswer accordingly
        if (optionAAnswer.ToLower().Contains("(correct)"))
        {
            correctAnswer = "A";
        }
        else if (optionBAnswer.ToLower().Contains("(correct)"))
        {
            correctAnswer = "B";
        }
        else if (optionCAnswer.ToLower().Contains("(correct)"))
        {
            correctAnswer = "C";
        }
        else
        {
            Debug.LogWarning("⚠️ No correct answer marked, defaulting to A.");
            correctAnswer = "A"; // fallback if none found
        }

        // ✅ Optional: Clean "(Correct)" from display text (if you want clean UI)
        optionAAnswer = optionAAnswer.Replace("(Correct)", "").Trim();
        optionBAnswer = optionBAnswer.Replace("(Correct)", "").Trim();
        optionCAnswer = optionCAnswer.Replace("(Correct)", "").Trim();

        // You can now optionally show the actual text if needed like:
        // optionAWorldSpaceText.text = optionAAnswer; etc.

        Debug.Log($"✅ Correct Answer Set To: {correctAnswer}");
    }


    // This function can be used to check if the vehicle selects the correct answer
    // For example, trigger colliders on the vehicle and check if it went through the correct option
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(correctAnswer))
        {
            questionManager.HandleAnswerAndNextQuestion(true);
        }
        else if (other.CompareTag("A") || other.CompareTag("B") || other.CompareTag("C"))
        {
            questionManager.HandleAnswerAndNextQuestion(false);
        }
    }

}
