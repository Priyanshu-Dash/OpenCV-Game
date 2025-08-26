using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartSceneManager : MonoBehaviour
{
    public Button inClassButton;
    public Button atHomeButton;

    void Start()
    {
        inClassButton.onClick.AddListener(() => LoadScene("InClassScene"));
        atHomeButton.onClick.AddListener(() => LoadScene("AtHome"));
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
