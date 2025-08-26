using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InClassUIManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform contentPanel;
    public GameObject studentItemPrefab;
    public TMP_Text currentStudentText;
    public TMP_Text totalStudentsText;

    [Header("Sprites")]
    public Sprite playedSprite;
    public Sprite notPlayedSprite;

    private List<StudentProfile> loadedStudents = new List<StudentProfile>();
    private List<GameObject> studentUIItems = new List<GameObject>();
    private int currentIndex = -1;

    private void Start()
    {
        LoadStudentsOfGrade7A();
        ResumeOrStartSession();
    }

    void LoadStudentsOfGrade7A()
    {
        TextAsset jsonData = Resources.Load<TextAsset>("StudentReports/Student_Reports");
        if (jsonData == null)
        {
            Debug.LogError("❌ Could not find Student_Reports.json");
            return;
        }

        StudentReportWrapper wrapper = JsonUtility.FromJson<StudentReportWrapper>(jsonData.text);
        loadedStudents = wrapper.students
            .Where(s => s.grade.ToUpper() == "7A")
            .OrderBy(s => s.studentName)
            .ToList();

        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < loadedStudents.Count; i++)
        {
            GameObject item = Instantiate(studentItemPrefab, contentPanel);
            studentUIItems.Add(item);

            // Setup student name
            TMP_Text nameText = item.GetComponentInChildren<TMP_Text>();
            nameText.text = $"<b><size=38>{loadedStudents[i].studentName}</size></b>\n<size=28>Roll - {loadedStudents[i].studentId}</size>";

            // Set default sprite (not played)
            Image bgImage = item.GetComponent<Image>();
            bgImage.sprite = notPlayedSprite;
        }

        totalStudentsText.text = $"{loadedStudents.Count} students";
    }

    void ResumeOrStartSession()
    {
        if (PlayerPrefs.HasKey("InClass_CurrentIndex"))
        {
            currentIndex = PlayerPrefs.GetInt("InClass_CurrentIndex", 0) + 1;
        }
        else
        {
            currentIndex = 0;
        }

        if (currentIndex < loadedStudents.Count)
        {
            ShowCurrentStudent();
        }
        else
        {
            currentStudentText.text = "✅ All students completed.";
        }
    }

    void ShowCurrentStudent()
    {
        var student = loadedStudents[currentIndex];
        currentStudentText.text = $"<b><size=38>{student.studentName}</size></b>\n<size=28>Roll - {student.studentId}</size>";

        // Update sprite for the student who just finished playing (if not first)
        if (currentIndex > 0 && currentIndex - 1 < studentUIItems.Count)
        {
            Image previousImage = studentUIItems[currentIndex - 1].GetComponent<Image>();
            previousImage.sprite = playedSprite;
        }

        // Save progress
        PlayerPrefs.SetInt("InClass_CurrentIndex", currentIndex);
        PlayerPrefs.Save();
    }
}