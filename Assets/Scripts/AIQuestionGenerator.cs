using System.Collections.Generic;
using UnityEngine;

public static class AIQuestionGenerator
{
    public static AIQuestion GenerateQuestion(string topic, string difficulty, int grade)
    {
        // Simulated question — later replace with Gemini API
        if (topic.ToLower().Contains("decimal"))
        {
            return new AIQuestion
            {
                topic = topic,
                difficulty = difficulty,
                gradeLevel = grade,
                question = "What is 12.6 ÷ 1.2?",
                options = new List<string> { "10.5", "11.2", "9.6", "10.1" },
                correctAnswer = "10.5"
            };
        }

        return new AIQuestion
        {
            topic = topic,
            difficulty = difficulty,
            gradeLevel = grade,
            question = "What is the mode of the numbers: 3, 4, 4, 5, 6?",
            options = new List<string> { "3", "4", "5", "6" },
            correctAnswer = "4"
        };
    }
}
