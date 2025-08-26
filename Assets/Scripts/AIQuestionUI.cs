using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AIQuestionUI : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public List<Button> optionButtons;

    [Header("Test Mode (Optional)")]
    public bool testMode;
    public AIQuestion testQuestion;

    private void Start()
    {
        if (testMode && testQuestion != null)
        {
            Setup(testQuestion);
        }
    }

    public void Setup(AIQuestion question)
    {
        questionText.text = question.question;
        var correctAnswer = question.correctAnswer;

        for (int i = 0; i < optionButtons.Count; i++)
        {
            var btn = optionButtons[i];
            var label = btn.GetComponentInChildren<TextMeshProUGUI>();
            label.text = question.options[i];

            string answer = question.options[i];
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => CheckAnswer(answer, correctAnswer));
        }
    }

    private void CheckAnswer(string selected, string correct)
    {
        Debug.Log(selected == correct ? "✅ Correct!" : "❌ Incorrect!");
    }
}
