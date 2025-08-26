using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 10f;    // Speed of forward movement
    public float leanSpeed = 6f;        // Speed of continuous leaning left/right
    public float maxLeanAngle = 30f;    // How much to tilt the spaceship on z-axis
    public float leanSmoothTime = 0.1f; // Smooth transition duration
    public float xLimit = 5f;           // Horizontal movement limit (±5 units)

    private float leanDirection = 0f;   // -1 for left, 1 for right, 0 for no leaning
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Time.timeScale = 1f;
    }

    private void FixedUpdate()
    {
        // Move forward
        Vector3 forwardMovement = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = rb.position + forwardMovement;

        // Move left/right based on input
        if (leanDirection != 0)
        {
            Vector3 leanMovement = Vector3.right * leanDirection * leanSpeed * Time.fixedDeltaTime;
            newPosition += leanMovement;
        }

        // Clamp X position within limits
        newPosition.x = Mathf.Clamp(newPosition.x, -xLimit, xLimit);
        rb.MovePosition(newPosition);

        // Smooth Z-axis lean
        float targetZ = -leanDirection * maxLeanAngle;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime / leanSmoothTime);
    }

    private void Update()
    {
        // Input can go here if needed
    }

    public void TurnLeft()
    {
        leanDirection = -1f;
    }

    public void TurnRight()
    {
        leanDirection = 1f;
    }

    public void Stand()
    {
        leanDirection = 0f;
    }

    public void LeanLeft()
    {
        leanDirection = -1f;
    }

    public void LeanRight()
    {
        leanDirection = 1f;
    }

    public void Run() { }
    public void Jump() { }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.GameOver();
        }
    }

    public void playagain()
    {
        SceneManager.LoadScene(1);
    }
}
