using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StudentLoader : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField studentNameInput;
    public Button loadButton;
    public TMP_Text studentNameDisplay;
    public TMP_Text invalidstudentNameDisplay;
    public GameObject GameCuration;
    public HomeUIManager onboardingManager;

    private void Start()
    {
        GameCuration.SetActive(false);
        if (loadButton != null)
        {
            loadButton.onClick.AddListener(OnLoadStudentClicked);
        }
    }

    private void OnLoadStudentClicked()
    {
        string inputName = studentNameInput.text.Trim();

        if (string.IsNullOrEmpty(inputName))
        {
            Debug.LogWarning("⚠ Please enter a valid student name.");
            return;
        }

        var profile = GeneoReportParser.LoadProfileById(inputName);

        if (profile != null && StudentProfileManager.Instance != null)
        {
            StudentProfileManager.Instance.LoadStudent(profile);
            Debug.Log($"✅ Loaded profile for: {profile.studentName}");

            if (studentNameDisplay != null)
                studentNameDisplay.text = $"{profile.studentName}";
            GameCuration.SetActive(true);

            // ✅ Call onboarding panel AFTER student is loaded
            if (onboardingManager != null)
                onboardingManager.OnboardingPanel();
        }
        else
        {
            Debug.LogWarning($"❌ Could not load profile for: {inputName}");
            if (invalidstudentNameDisplay != null)
                invalidstudentNameDisplay.text = $"No Student Found";
        }
    }
}
