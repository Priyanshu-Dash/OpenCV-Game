using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StudentProfile
{
    public string studentId;           // ← From StudentEnrollmentID
    public string studentName;         // ← From FullName
    public string grade;               // ← From ClassName
    public string section;             // ← From Section
    public string score;               // Optional, leave blank if unknown
    public List<string> learningGaps;  // ← Set from DeepLink manually for now
}
