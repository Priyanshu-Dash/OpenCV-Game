using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 10f;
    public float horizontalSpeed = 5f;
    public float horizontalLimit = 5f;

    [Header("Tilt Settings")]
    public float maxTiltZ = 25f; // Max lean angle
    public float tiltSmoothSpeed = 5f;

    private float tiltInput;

    void Update()
    {
        // Move spaceship forward continuously
        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        // Get accelerometer input for tilt-based movement
        tiltInput = Input.acceleration.x;

        // Calculate target X position
        float targetX = transform.position.x + tiltInput * horizontalSpeed * Time.deltaTime;

        // Clamp X within limits
        targetX = Mathf.Clamp(targetX, -horizontalLimit, horizontalLimit);

        // Apply movement on X axis
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

        // Calculate target Z-axis rotation (leaning)
        float targetZRotation = -tiltInput * maxTiltZ;

        // Smoothly rotate the spaceship around Z axis
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZRotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltSmoothSpeed);
    }
}
