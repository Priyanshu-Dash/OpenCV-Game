using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenOrientationManager : MonoBehaviour
{
    public string givenSceneName;

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == givenSceneName)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
        else if(sceneName == "")
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
    }
}
