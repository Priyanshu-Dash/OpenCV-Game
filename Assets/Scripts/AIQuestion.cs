using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AIQuestion
{
    public string question;
    public List<string> options;
    public string correctAnswer;
    public string topic;
    public string difficulty;
    public int gradeLevel;
}
