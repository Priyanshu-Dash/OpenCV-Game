using System.Collections.Generic;
using UnityEngine;

public static class AIQuestionParser
{
    public static AIQuestion FromGeminiText(string text)
    {
        // You can enhance this with regex or format parsing
        // Example input:
        /*
        Question: What is 5.6 ÷ 0.7?
        A. 7
        B. 8
        C. 6
        D. 9
        Answer: B
        */

        string[] lines = text.Split('\n');
        string q = lines[0].Replace("Question:", "").Trim();

        var options = new List<string>
        {
            lines[1].Substring(2).Trim(),
            lines[2].Substring(2).Trim(),
            lines[3].Substring(2).Trim(),
            lines[4].Substring(2).Trim()
        };

        string correct = lines[5].Split(':')[1].Trim();
        string correctAnswer = options[correct[0] - 'A'];

        return new AIQuestion
        {
            question = q,
            options = options,
            correctAnswer = correctAnswer
        };
    }
}
