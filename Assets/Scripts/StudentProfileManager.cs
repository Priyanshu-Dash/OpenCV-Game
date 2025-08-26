using UnityEngine;
using System.Collections.Generic;

public class StudentProfileManager : MonoBehaviour
{
    public static StudentProfileManager Instance;

    [Header("DEBUG: Current Student Info")]
    public StudentProfile CurrentStudentProfile;
    public StudentProfile CurrentStudent { get; private set; }

    [Header("Optional UI Fields")]
    public TMPro.TextMeshProUGUI studentNameText;
    public TMPro.TextMeshProUGUI gradeText;
    public TMPro.TextMeshProUGUI sectionText;
    public TMPro.TextMeshProUGUI learningGapsText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        string studentId = DeepLinkManager.CurrentStudentID;

        if (!string.IsNullOrEmpty(studentId))
        {
            StudentProfile profile = GeneoReportParser.LoadProfileById(studentId);
            if (profile != null)
            {
                profile.learningGaps = DeepLinkManager.CuratedLearningGaps; // For debug if needed
                LoadStudent(profile);
            }
            else
            {
                Debug.LogWarning("⚠️ No student profile found for ID: " + studentId);
            }
        }
    }

    public void LoadStudent(StudentProfile profile)
    {
        CurrentStudent = profile;
        CurrentStudentProfile = profile;

        Debug.Log($"📘 Loaded: {profile.studentName} | Grade: {profile.grade} | Section: {profile.section}");

        if (studentNameText != null) studentNameText.text = profile.studentName;
        if (gradeText != null) gradeText.text = profile.grade;
        if (sectionText != null) sectionText.text = profile.section;
        if (learningGapsText != null)
            learningGapsText.text = "Gaps: " + string.Join(", ", profile.learningGaps);
    }


    public StudentProfile GetStudentProfile()
    {
        return CurrentStudentProfile;
    }
}
