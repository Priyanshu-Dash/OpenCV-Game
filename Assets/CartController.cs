using UnityEngine;
using UnityEngine.SceneManagement;

public class CartController : MonoBehaviour
{
    public float forwardSpeed = 10f;    // Speed of forward movement (Y-axis in 2D)
    public float leanSpeed = 6f;        // Speed of left/right movement (X-axis)
    public float maxLeanAngle = 30f;    // Tilt angle (for visual rotation only)
    public float leanSmoothTime = 0.1f; // How smooth the lean transition is
    public float xLimit = 5f;           // Horizontal clamp limit

    private float leanDirection = 0f;   // -1 = left, 1 = right
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Time.timeScale = 1f;
    }

    void FixedUpdate()
    {
        // Forward (upward) movement
        Vector2 forwardMovement = Vector2.up * forwardSpeed * Time.fixedDeltaTime;
        Vector2 newPosition = rb.position + forwardMovement;

        // Horizontal lean movement
        if (leanDirection != 0)
        {
            Vector2 leanMovement = Vector2.right * leanDirection * leanSpeed * Time.fixedDeltaTime;
            newPosition += leanMovement;
        }

        // Clamp X position
        newPosition.x = Mathf.Clamp(newPosition.x, -xLimit, xLimit);
        rb.MovePosition(newPosition);

        // Lean visual tilt (Z-axis rotation only for effect)
        float targetZ = -leanDirection * maxLeanAngle;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime / leanSmoothTime);
    }

    public void TurnLeft() => leanDirection = -1f;
    public void TurnRight() => leanDirection = 1f;
    public void Stand() => leanDirection = 0f;
    public void LeanLeft() => leanDirection = -1f;
    public void LeanRight() => leanDirection = 1f;
    public void Run() { }
    public void Jump() { }

    void OnCollisionEnter2D(Collision2D collision)
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
