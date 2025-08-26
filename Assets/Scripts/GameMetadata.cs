using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMetadata
{
    public string gameId;
    public string title;
    public List<string> learningOutcomes;
    public List<string> keywords; // ✅ New field
    public string difficulty;
    public Dictionary<string, string> configurableParameters;
}
