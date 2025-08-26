using UnityEngine;
using TMPro;
using UnityEngine.UI; // ✅ Required for Button type

public class HomeUIManager : MonoBehaviour
{
    public GameObject loginPage;
    public GameObject inputName;
    public GameObject Onboarding;
    public GameObject Home;
    public GameObject Profile;
    public GameObject learningProgress;
    public GameObject Settings;
    public GameObject LOP;
    public Button open;
    public Button close;

    void Start()
    {
        loginPage.SetActive(false);
        inputName.SetActive(false);
        Onboarding.SetActive(false);
        Home.SetActive(true);
        Profile.SetActive(false);
        learningProgress.SetActive(false);  
        open.gameObject.SetActive(true);
        Settings.SetActive(false);
    }

    public void openLoginPage()
    {
        loginPage.SetActive(true);
        inputName.SetActive(false);
        Onboarding.SetActive(false);
        Home.SetActive(false);
        Profile.SetActive(false);
        learningProgress.SetActive(false);
        Settings.SetActive(false);
    }
    public void openInputName()
    {
        loginPage.SetActive(false);
        inputName.SetActive(true);
        Onboarding.SetActive(false);
        Home.SetActive(false);
        Profile.SetActive(false);
        learningProgress.SetActive(false);
        Settings.SetActive(false);
    }
    public void OnboardingPanel()
    {
        if (StudentProfileManager.Instance != null && StudentProfileManager.Instance.CurrentStudent != null)
        {
            loginPage.SetActive(false);
            inputName.SetActive(false);
            Onboarding.SetActive(true);
            Home.SetActive(false);
            Profile.SetActive(false);
            learningProgress.SetActive(false);
            Settings.SetActive(false);
        }
        else
        {
            Debug.LogWarning("⚠ Cannot open Onboarding panel. No valid student profile is loaded.");
        }
    }

    public void openHomePanel()
    {
        loginPage.SetActive(false);
        inputName.SetActive(false);
        Onboarding.SetActive(false);
        Home.SetActive(true);
        Profile.SetActive(false);
        learningProgress.SetActive(false);
        Settings.SetActive(false);
    }

    public void openProfilePanel()
    {
        loginPage.SetActive(false);
        inputName.SetActive(false);
        Onboarding.SetActive(false);
        Home.SetActive(false);
        Profile.SetActive(true);
        learningProgress.SetActive(false);
        Settings.SetActive(false);
    }

    public void openLearningProgressPanel()
    {
        loginPage.SetActive(false);
        inputName.SetActive(false);
        Onboarding.SetActive(false);
        Home.SetActive(false);
        Profile.SetActive(false);
        learningProgress.SetActive(true);
        Settings.SetActive(false);
    }
    public void openSettings()
    {
        loginPage.SetActive(false);
        inputName.SetActive(false);
        Onboarding.SetActive(false);
        Home.SetActive(false);
        Profile.SetActive(false);
        learningProgress.SetActive(false);
        Settings.SetActive(true);
    }

    public void open_lo()
    {
        LOP.SetActive(true);
        close.gameObject.SetActive(true);
        open.gameObject.SetActive(false);
    }

    public void close_lo()
    {
        LOP.SetActive(false);
        close.gameObject.SetActive(false);
        open.gameObject.SetActive(true);
    }
}
