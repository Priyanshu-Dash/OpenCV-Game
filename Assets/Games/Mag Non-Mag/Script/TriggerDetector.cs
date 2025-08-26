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
    
    [Header("Magnetic Detection")]
    [SerializeField] private MagneticGameManager magneticGameManager; // Reference to the game manager
    
    [Header("Magnetic Movement")]
    [SerializeField] private float magneticForce = 50f;        // Speed of magnetic attraction (increased for fast movement)
    [SerializeField] private float magneticRange = 10f;       // Range of magnetic effect
    [SerializeField] private bool enableMagneticMovement = true; // Toggle for magnetic movement
    
    private bool isCountingDown = false;
    private float currentTime = 0f;
    private string currentObjectName = "";
    private string currentObjectTag = "";
    private bool isCurrentObjectMagnetic = false; // Track if current object is magnetic
    private GameObject currentObject; // Reference to the current object in trigger
    private GameObject magnateObject; // Reference to the magnate object
    private bool isMovingToMagnate = false; // Track if object is currently moving
    private bool isObjectActivated = false; // Track if the current object is activated

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
        
        // Find MagneticGameManager if not assigned
        if (magneticGameManager == null)
        {
            magneticGameManager = FindObjectOfType<MagneticGameManager>();
            if (magneticGameManager == null)
            {
                Debug.LogWarning("TriggerDetector: No MagneticGameManager found! Magnetic detection will not work.");
            }
        }
        
        // Find the magnate object
        FindMagnateObject();
    }
    
    void FindMagnateObject()
    {
        magnateObject = GameObject.FindGameObjectWithTag("magnate");
        if (magnateObject == null)
        {
            Debug.LogWarning("TriggerDetector: No GameObject with 'magnate' tag found! Magnetic movement will not work.");
        }
        else
        {
            Debug.Log("TriggerDetector: Found magnate object: " + magnateObject.name + " at position: " + magnateObject.transform.position);
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
        
        // Handle magnetic movement - CONTINUOUS until perfect overlap
        if (enableMagneticMovement && isMovingToMagnate && currentObject != null && magnateObject != null)
        {
            MoveObjectToMagnate();
        }
    }
    
    void MoveObjectToMagnate()
    {
        if (currentObject == null || magnateObject == null) return;
        
        float distance = Vector3.Distance(currentObject.transform.position, magnateObject.transform.position);
        
        // Check if we're close enough to snap to exact position
        if (distance <= 0.01f)
        {
            // Snap to exact position for perfect overlap
            currentObject.transform.position = magnateObject.transform.position;
            isMovingToMagnate = false;
            Debug.Log("Object completely stuck to magnate - perfect overlap achieved at position: " + currentObject.transform.position);
            return;
        }
        
        // Use MoveTowards for more precise movement towards target
        currentObject.transform.position = Vector3.MoveTowards(
            currentObject.transform.position, 
            magnateObject.transform.position, 
            magneticForce * Time.deltaTime
        );
        
        // Debug: Log current position to see progress
        if (Time.frameCount % 60 == 0) // Log every 60 frames (about once per second)
        {
            Debug.Log("Moving to magnate - Current: " + currentObject.transform.position + 
                     " Target: " + magnateObject.transform.position + 
                     " Distance: " + distance.ToString("F3"));
        }
    }

    // Called when another Collider2D enters the trigger zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Start the countdown timer
        StartTimer(other.gameObject.name, other.tag);
    }
    
    // Called when another Collider2D exits the trigger zone
    private void OnTriggerExit2D(Collider2D other)
    {
        // Only reset timer if object hasn't been activated yet
        // This allows activated objects to continue moving towards the magnet
        if (!isObjectActivated)
        {
            ResetTimer();
        }
    }

    // Start the countdown timer
    private void StartTimer(string objectName, string objectTag)
    {
        // Reset activation state for new object
        isObjectActivated = false;
        
        currentObjectName = objectName;
        currentObjectTag = objectTag;
        currentTime = 0f;
        isCountingDown = true;
        
        // Store reference to the current object
        currentObject = GameObject.Find(objectName);
        
        // Detect if the object is magnetic
        DetectMagneticProperties();
        
        // Show the timer images and reset fill
        if (backgroundImage != null && fillImage != null)
        {
            backgroundImage.gameObject.SetActive(true);
            fillImage.gameObject.SetActive(true);
            fillImage.fillAmount = 1f;
        }
        
        Debug.Log("Started timer for: " + objectName + " with tag: " + objectTag + " (Magnetic: " + isCurrentObjectMagnetic + ")");
    }
    
    // Reset the timer
    private void ResetTimer()
    {
        isCountingDown = false;
        currentTime = 0f;
        isMovingToMagnate = false; // Stop magnetic movement
        isObjectActivated = false; // Reset activation state
        
        // Hide the timer images
        if (backgroundImage != null && fillImage != null)
        {
            backgroundImage.gameObject.SetActive(false);
            fillImage.gameObject.SetActive(false);
        }
        
        Debug.Log("Timer reset - object left trigger zone");
    }
    
    // Detect if the current object is magnetic
    private void DetectMagneticProperties()
    {
        if (magneticGameManager == null)
        {
            Debug.LogWarning("Cannot detect magnetic properties - MagneticGameManager not found!");
            isCurrentObjectMagnetic = false;
            return;
        }
        
        // Get the object's sprite renderer or image component
        SpriteRenderer spriteRenderer = null;
        Image imageComponent = null;
        
        // Try to find SpriteRenderer first
        spriteRenderer = GameObject.Find(currentObjectName)?.GetComponent<SpriteRenderer>();
        
        // If no SpriteRenderer, try to find Image component
        if (spriteRenderer == null)
        {
            imageComponent = GameObject.Find(currentObjectName)?.GetComponent<Image>();
        }
        
        if (spriteRenderer != null)
        {
            // Check if the sprite is magnetic or non-magnetic
            isCurrentObjectMagnetic = IsSpriteMagnetic(spriteRenderer.sprite);
        }
        else if (imageComponent != null)
        {
            // Check if the image sprite is magnetic or non-magnetic
            isCurrentObjectMagnetic = IsSpriteMagnetic(imageComponent.sprite);
        }
        else
        {
            Debug.LogWarning("Could not find SpriteRenderer or Image component on object: " + currentObjectName);
            isCurrentObjectMagnetic = false;
        }
    }
    
    // Check if a sprite is magnetic by comparing it with the game manager's sprite arrays
    private bool IsSpriteMagnetic(Sprite sprite)
    {
        if (sprite == null || magneticGameManager == null)
            return false;
            
        // Use the public method from MagneticGameManager
        return magneticGameManager.IsSpriteMagnetic(sprite);
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
        
        // Start magnetic movement if enabled and magnate exists
        if (enableMagneticMovement && magnateObject != null && currentObject != null)
        {
            isObjectActivated = true; // Mark object as activated
            StartMagneticMovement();
        }
        
        // Print the name of the object after the delay with magnetic information
        Debug.Log("Object stayed in trigger zone for " + countdownDuration + " seconds: " + currentObjectName + 
                  " (tag: " + currentObjectTag + ", Magnetic: " + isCurrentObjectMagnetic + ")" +
                  " - Object activated and will continue moving towards magnet");
    }
    
    void StartMagneticMovement()
    {
        if (currentObject == null || magnateObject == null) return;
        
        isMovingToMagnate = true;
        Debug.Log("Starting magnetic movement for: " + currentObject.name + " towards magnate at position: " + magnateObject.transform.position);
        
        // You can add additional effects here like:
        // - Particle effects
        // - Sound effects
        // - Visual feedback
    }
    
    // Public method to get current object's magnetic status
    public bool IsCurrentObjectMagnetic()
    {
        return isCurrentObjectMagnetic;
    }
    
    // Public method to get current object's name
    public string GetCurrentObjectName()
    {
        return currentObjectName;
    }
    
    // Public method to get current object's tag
    public string GetCurrentObjectTag()
    {
        return currentObjectTag;
    }
    
    // Public method to manually start magnetic movement
    public void StartMagneticMovementManual(GameObject targetObject)
    {
        if (targetObject == null || magnateObject == null) return;
        
        currentObject = targetObject;
        StartMagneticMovement();
    }
    
    // Public method to stop magnetic movement
    public void StopMagneticMovement()
    {
        isMovingToMagnate = false;
        Debug.Log("Magnetic movement stopped");
    }

    // Public method to manually reset the activation state for a new object
    public void ResetObjectActivation()
    {
        isObjectActivated = false;
        Debug.Log("Object activation state reset for: " + currentObjectName);
    }
}
