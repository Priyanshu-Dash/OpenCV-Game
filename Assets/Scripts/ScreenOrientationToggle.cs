using UnityEngine;

public class ScreenOrientationToggle : MonoBehaviour
{
    public GameObject landscape;  // To be deactivated in Portrait
    public GameObject portrait;  // To be deactivated in Landscape

    void Start()
    {
        UpdateOrientation();
    }

    void Update()
    {
        // Continuously check in case orientation changes during runtime
        UpdateOrientation();
    }

    void UpdateOrientation()
    {
        if (Screen.width > Screen.height)
        {
            // Landscape
            if (landscape != null) landscape.SetActive(true);
            if (portrait != null) portrait.SetActive(false);
        }
        else
        {
            // Portrait
            if (landscape != null) landscape.SetActive(false);
            if (portrait != null) portrait.SetActive(true);
        }
    }
}
