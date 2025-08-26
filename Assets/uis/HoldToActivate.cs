using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoldToActivate : MonoBehaviour
{
    public GameObject transitionScreen1; // 👈 Assign in Inspector (the GameObject to activate)
    public GameObject transitionScreen2;
    public float holdDuration = 5f; // Seconds to hold
    public float delayAfterActivation = 3f; // Seconds to wait after activation before scene change
    public string sceneToLoad = "SampleScene";

    private float holdTimer = 0f;
    private bool isActivated = false;

    
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        transitionScreen1.SetActive(false);
    }
    public IEnumerator ActivateAndLoadScene()
    {

        if (transitionScreen2 != null)
        {
            transitionScreen2.SetActive(true);
        }
        else
        {
            Debug.LogWarning("⚠️ No transition screen assigned.");
        }

        yield return new WaitForSeconds(delayAfterActivation);

        SceneManager.LoadScene(sceneToLoad);
    }
    public void Backtoscene()
    {
        StartCoroutine(ActivateAndLoadScene());
    }
}
