using UnityEngine;
using UnityEngine.UI;
using Mediapipe.Unity;

public class HandImagePositioner : MonoBehaviour
{
    [Header("Hand Tracking Setup")]
    [SerializeField] private MultiHandLandmarkListAnnotation hands; // Reference to MediaPipe hands
    [SerializeField] private Camera mainCamera; // Main camera reference
    
    [Header("Image GameObject Settings")]
    [SerializeField] private GameObject imageGameObject; // The image GameObject to position
    [SerializeField] private bool useCanvasImage = true; // If true, treats as UI Image, if false as SpriteRenderer
    
    [Header("Hand Tracking Settings")]
    [SerializeField] private int handIndex = 0; // Which hand to track (0 = first hand, 1 = second hand)
    [SerializeField] private int landmarkIndex = 9; // Which landmark to track (9 = middle finger MCP joint, good for palm center)
    
    [Header("Movement Settings")]
    [SerializeField] private float followSpeed = 10f; // How fast the image follows the hand
    [SerializeField] private bool smoothMovement = true; // Enable smooth movement
    [SerializeField] private Vector3 offset = Vector3.zero; // Offset from hand position
    
    [Header("Visibility Settings")]
    [SerializeField] private bool hideWhenHandNotDetected = true; // Hide image when hand is not detected
    
    private RectTransform rectTransform; // For UI Image
    private Transform imageTransform; // For regular GameObject
    private Image uiImage; // For UI Image component
    private bool isInitialized = false;
    
    void Start()
    {
        InitializeComponents();
    }
    
    void InitializeComponents()
    {
        if (imageGameObject == null)
        {
            Debug.LogError("HandImagePositioner: No image GameObject assigned!");
            return;
        }
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("HandImagePositioner: No main camera found!");
                return;
            }
        }
        
        // Check if it's a UI Image or regular GameObject
        uiImage = imageGameObject.GetComponent<Image>();
        if (uiImage != null)
        {
            useCanvasImage = true;
            rectTransform = imageGameObject.GetComponent<RectTransform>();
        }
        else
        {
            useCanvasImage = false;
            imageTransform = imageGameObject.transform;
        }
        
        isInitialized = true;
        
        // Initially hide if setting is enabled
        if (hideWhenHandNotDetected)
        {
            imageGameObject.SetActive(false);
        }
    }
    
    void Update()
    {
        if (!isInitialized || hands == null || imageGameObject == null)
            return;
            
        // Simple approach like FollowHand.cs - just check if hands exist and get the specified hand
        if (hands.count == 0 || handIndex >= hands.count || hands[handIndex] == null)
        {
            // Hide the image if hand is not detected and setting is enabled
            if (hideWhenHandNotDetected && imageGameObject.activeSelf)
            {
                Debug.Log("HandImagePositioner: Hand lost - hiding image");
                imageGameObject.SetActive(false);
            }
            return;
        }

        // Get landmark point for the specified hand
        var point = hands[handIndex][landmarkIndex];

        if (point == null || !point.gameObject.activeSelf)
        {
            // Hide the image if landmark is not active
            if (hideWhenHandNotDetected && imageGameObject.activeSelf)
            {
                Debug.Log("HandImagePositioner: Hand landmark not active - hiding image");
                imageGameObject.SetActive(false);
            }
            return;
        }

        // Show the image if it was hidden
        if (hideWhenHandNotDetected && !imageGameObject.activeSelf)
        {
            Debug.Log("HandImagePositioner: Hand detected - showing image");
            imageGameObject.SetActive(true);
        }
        
        // Position the image at the hand location
        PositionImageAtHand(point.transform.position + offset);
    }
    

    
    void PositionImageAtHand(Vector3 handWorldPosition)
    {
        Vector3 targetPosition = handWorldPosition;
        
        if (useCanvasImage && rectTransform != null)
        {
            // For UI Image - convert to canvas coordinates
            PositionUIImage(targetPosition);
        }
        else if (imageTransform != null)
        {
            // For regular GameObject - position in world space
            PositionWorldImage(targetPosition);
        }
    }
    
    void PositionUIImage(Vector3 worldPosition)
    {
        // Convert world position to screen position
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);
        
        // Make sure the point is in front of the camera
        if (screenPosition.z > 0)
        {
            // Convert screen position to Canvas position
            Vector2 canvasPosition;
            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            
            if (canvas != null)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    screenPosition,
                    canvas.worldCamera,
                    out canvasPosition
                );
                
                // Apply movement (smooth or instant)
                if (smoothMovement)
                {
                    rectTransform.localPosition = Vector3.Lerp(
                        rectTransform.localPosition, 
                        canvasPosition, 
                        Time.deltaTime * followSpeed
                    );
                }
                else
                {
                    rectTransform.localPosition = canvasPosition;
                }
            }
        }
    }
    
    void PositionWorldImage(Vector3 worldPosition)
    {
        // For regular GameObjects in world space
        Vector3 targetPos = worldPosition;
        targetPos.z = imageTransform.position.z; // Maintain original Z depth
        
        // Apply movement (smooth or instant)
        if (smoothMovement)
        {
            imageTransform.position = Vector3.Lerp(
                imageTransform.position, 
                targetPos, 
                Time.deltaTime * followSpeed
            );
        }
        else
        {
            imageTransform.position = targetPos;
        }
    }
    
    // Public methods for runtime configuration
    public void SetHandIndex(int newHandIndex)
    {
        if (newHandIndex >= 0)
        {
            handIndex = newHandIndex;
        }
    }
    
    public void SetLandmarkIndex(int newIndex)
    {
        if (newIndex >= 0 && newIndex < 21) // MediaPipe has 21 hand landmarks
        {
            landmarkIndex = newIndex;
        }
    }
    
    public void SetFollowSpeed(float newSpeed)
    {
        followSpeed = newSpeed;
    }
    
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}
