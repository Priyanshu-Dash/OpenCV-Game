using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class FirebaseAuthManager : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TextMeshProUGUI statusText;

    public GameObject StartPanel;
    public GameObject signinPanel;
    public GameObject OnboardPanel;

    private FirebaseAuth auth;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        StartPanel.SetActive(true);
        signinPanel.SetActive(false);
        OnboardPanel.SetActive(false);
    }

    public void RegisterUser()
    {
        auth.CreateUserWithEmailAndPasswordAsync(emailInput.text, passwordInput.text)
            .ContinueWithOnMainThread(task => {
                if (task.IsCanceled || task.IsFaulted)
                {
                    statusText.text = "Registration failed: " + task.Exception?.Flatten().Message;
                    return;
                }

                // ✅ Correct: Get user from UserCredential
                FirebaseUser newUser = task.Result.User;
                statusText.text = "Registration successful: " + newUser.Email;
            });
    }

    public void LoginUser()
    {
        auth.SignInWithEmailAndPasswordAsync(emailInput.text, passwordInput.text)
            .ContinueWithOnMainThread(task => {
                if (task.IsCanceled || task.IsFaulted)
                {
                    statusText.text = "Login failed: User doesn't exist!";
                    return;
                }

                // ✅ Correct: Get user from UserCredential
                FirebaseUser user = task.Result.User;
                statusText.text = "Login successful: " + user.Email;
                OnboardPanel.SetActive(true);
            });
    }

    public void LogoutUser()
    {
        auth.SignOut();
        statusText.text = "User logged out.";
    }

    public void OpenStartPanel()
    {
        StartPanel.SetActive(true);
        signinPanel.SetActive(false);
        OnboardPanel.SetActive(false);
    }
    public void OpenSigninPanel()
    {
        StartPanel.SetActive(false);
        signinPanel.SetActive(true);
        OnboardPanel.SetActive(false);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene("AtHome");
    }
}
