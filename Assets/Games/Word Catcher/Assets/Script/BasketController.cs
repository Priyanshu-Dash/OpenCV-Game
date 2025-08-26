using UnityEngine;

public class BasketController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float movementRange = 8f;         // Max range from center (±value)
    public float smoothing = 5f;             // Lerp speed
    public bool allowMovement = false;       // Enable/disable movement

    private float targetX;

    /// <summary>
    /// Called from MediaPipeNoseBridge with normalized face X (0 to 1)
    /// </summary>
    /// <param name="normalizedFaceX">Normalized face X (0=left, 1=right)</param>
    public void UpdateBasketPosition(float normalizedFaceX)
    {
        if (!allowMovement) return;

        // Convert normalized X to world space
        float worldX = Mathf.Lerp(-movementRange, movementRange, normalizedFaceX);
        targetX = worldX;
    }

    void Update()
    {
        if (!allowMovement) return;

        Vector3 currentPos = transform.position;

        // Smoothly interpolate toward the target X position
        currentPos.x = Mathf.Lerp(currentPos.x, targetX, Time.deltaTime * smoothing);
        currentPos.x = Mathf.Clamp(currentPos.x, -movementRange, movementRange);

        transform.position = currentPos;
    }
}
