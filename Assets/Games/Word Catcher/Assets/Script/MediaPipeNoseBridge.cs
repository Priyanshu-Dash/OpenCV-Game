using UnityEngine;

public class MediaPipeNoseBridge : MonoBehaviour
{
    public BasketController basketController;     // Reference to your gameplay controller
    public static int screenWidth;
    public static int screenHeight;
    public string activeGameName = "";

    private float smoothedX = 0f;
    private float faceSmoothingSpeed = 5f;        // Smoothing speed for face movement

    void Start()
    {
        screenWidth = Screen.height;
        screenHeight = Screen.width;
    }

    /// <summary>
    /// Called from FaceLandmarkerRunner with face center X and dummy Y (ignored)
    /// </summary>
    public void OnReceiveFaceData(string faceCenterPos)
    {
        string[] parts = faceCenterPos.Split(',');

        if (parts.Length != 2)
            return;

        if (float.TryParse(parts[0], out float x))
        {
            // Normalize X from screen pixels to 0-1
            float normalizedX = Mathf.Clamp01(x / (float)screenWidth);

            // Apply smoothing
            smoothedX = Mathf.Lerp(smoothedX, normalizedX, Time.deltaTime * faceSmoothingSpeed);

            // Send to controller (you decide how to use smoothedX in world space)
            basketController.UpdateBasketPosition(smoothedX);
        }
    }
}
