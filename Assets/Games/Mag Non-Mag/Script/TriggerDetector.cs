using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TriggerDetector : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image backgroundImage;    // Background image for the radial timer
    [SerializeField] private Image fillImage;          // Fill image that decreases as timer progresses
    
    [Header("Timer Settings")]
    [SerializeField] private float countdownDuration = 3f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    private static readonly int IsActive = Animator.StringToHash("IsActive");
    private static readonly int Left = Animator.StringToHash("left");
    private static readonly int Right = Animator.StringToHash("right");
    
    private bool isCountingDown = false;
    private float currentTime = 0f;
    private string currentObjectName = "";
    private string currentObjectTag = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Make sure this GameObject has a Collider2D component set as trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("TriggerDetector: No Collider2D component found! Please add a Collider2D component.");
        }
        
        // Setup the radial timer images
        if (backgroundImage != null && fillImage != null)
        {
            // Make sure fill image is set to fill type
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Radial360;
            fillImage.fillOrigin = (int)Image.Origin360.Top;
            fillImage.fillAmount = 1f;
            
            // Hide both images initially
            backgroundImage.gameObject.SetActive(false);
            fillImage.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("TriggerDetector: Please assign both backgroundImage and fillImage in the inspector!");
        }
    }

    void Update()
    {
        // Update the timer if counting down
        if (isCountingDown)
        {
            currentTime += Time.deltaTime;
            
            // Update the fill amount (decrease from 1 to 0)
            if (fillImage != null)
            {
                float fillAmount = 1f - (currentTime / countdownDuration);
                fillImage.fillAmount = Mathf.Clamp01(fillAmount);
            }
            
            // Check if timer is complete
            if (currentTime >= countdownDuration)
            {
                CompleteTimer();
            }
        }
    }

    // Called when another Collider2D enters the trigger zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Start the countdown timer
        StartTimer(other.gameObject.name, other.tag);
    }

    // Start the countdown timer
    private void StartTimer(string objectName, string objectTag)
    {
        currentObjectName = objectName;
        currentObjectTag = objectTag;
        currentTime = 0f;
        isCountingDown = true;
        
        // Show the timer images and reset fill
        if (backgroundImage != null && fillImage != null)
        {
            backgroundImage.gameObject.SetActive(true);
            fillImage.gameObject.SetActive(true);
            fillImage.fillAmount = 1f;
        }
        
        Debug.Log("Started timer for: " + objectName + " with tag: " + objectTag);
    }

    // Complete the timer
    private void CompleteTimer()
    {
        isCountingDown = false;
        
        // Hide the timer images
        if (backgroundImage != null && fillImage != null)
        {
            backgroundImage.gameObject.SetActive(false);
            fillImage.gameObject.SetActive(false);
        }
        
        // Set the appropriate animator bool based on the object's tag
        if (animator != null)
        {
            if (currentObjectTag == "left")
            {
                animator.SetBool(Left, true);
                animator.SetBool(Right, false);
                Debug.Log("Set animator 'left' to true, 'right' to false");
            }
            else if (currentObjectTag == "right")
            {
                animator.SetBool(Right, true);
                animator.SetBool(Left, false);
                Debug.Log("Set animator 'right' to true, 'left' to false");
            }
        }
        
        // Print the name of the object after the delay
        Debug.Log("Object stayed in trigger zone for " + countdownDuration + " seconds: " + currentObjectName + " (tag: " + currentObjectTag + ")");
    }
}
