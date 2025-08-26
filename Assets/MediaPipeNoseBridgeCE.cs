using UnityEngine;

public class MediaPipeNoseBridgeCE : MonoBehaviour
{
    public BasketController basketController;
    public static float screenWidth;
    public static float screenHeight;

    private float currentX = 0f;
    private float targetX = 0f;

    [Range(0f, 100f)]
    public float responsiveness = 20f; // Increase this for snappier movement

    void Start()
    {
        screenWidth = Screen.height*0.2f;
        screenHeight = Screen.width*0.2f;
    }

    void Update()
    {
        // Quick linear interpolation toward target
        currentX = Mathf.Lerp(currentX, targetX, responsiveness * Time.deltaTime);

        // Update the basket position in world space
        basketController.UpdateBasketPosition(currentX);
    }

    public void OnReceiveFaceData(string faceCenterPos)
    {
        string[] parts = faceCenterPos.Split(',');

        if (parts.Length != 2)
            return;

        if (float.TryParse(parts[0], out float x))
        {
            // Convert pixel to normalized 0-1 range and flip
            float normalizedX = Mathf.Clamp01(x / (float)screenWidth);
            targetX = normalizedX;
        }
    }
}
