using UnityEngine;

public class AuthenticationPanels : MonoBehaviour
{
    public GameObject LoginPanel;
    public GameObject SigninPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoginPanel.SetActive(true);
        SigninPanel.SetActive(false);
    }

    public void OpenLoginPanel()
    {
        LoginPanel.SetActive(true);
        SigninPanel.SetActive(false);
    }
    public void OpenSigninPanel()
    {
        LoginPanel.SetActive(false);
        SigninPanel.SetActive(true);
    }
}
