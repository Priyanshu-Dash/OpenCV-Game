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
    [SerializeField] private bool hideWhenHandOutOfCamera = true; // Hide image when hand is outside camera view
    [SerializeField] private bool hideWhenHandBehindCamera = true; // Hide image when hand is behind camera
    [SerializeField] private bool enableDebugLogs = true; // Enable debug logging
    
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
        {
            if (enableDebugLogs)
                Debug.Log("HandImagePositioner: Not initialized or missing components");
            return;
        }
        
        // Debug hand count
        if (enableDebugLogs)
            Debug.Log($"HandImagePositioner: Hands detected: {hands.count}");
            
        // Check if hands are detected
        if (hands.count == 0)
        {
            HideImageIfNeeded("No hands detected");
            return;
        }
        
        if (handIndex >= hands.count)
        {
            HideImageIfNeeded($"Hand index {handIndex} out of range (max: {hands.count - 1})");
            return;
        }
        
        if (hands[handIndex] == null)
        {
            HideImageIfNeeded($"Hand at index {handIndex} is null");
            return;
        }

        // Get landmark point for the specified hand
        var point = hands[handIndex][landmarkIndex];

        if (point == null)
        {
            HideImageIfNeeded($"Landmark {landmarkIndex} is null");
            return;
        }
        
        if (!point.gameObject.activeSelf)
        {
            HideImageIfNeeded($"Landmark {landmarkIndex} is not active");
            return;
        }

        // Check if hand is visible in camera view
        Vector3 handWorldPosition = point.transform.position + offset;
        bool isVisible = IsHandVisibleInCamera(handWorldPosition);
        
        if (enableDebugLogs)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(handWorldPosition);
            Debug.Log($"HandImagePositioner: Hand position - World: {handWorldPosition}, Screen: {screenPos}, Visible: {isVisible}");
        }
        
        if (!isVisible)
        {
            HideImageIfNeeded("Hand not visible in camera");
            return;
        }

        // Show the image if it was hidden and hand is now visible
        if (hideWhenHandNotDetected && !imageGameObject.activeSelf)
        {
            if (enableDebugLogs)
                Debug.Log("HandImagePositioner: Hand detected and visible - showing image");
            imageGameObject.SetActive(true);
        }
        
        // Position the image at the hand location
        PositionImageAtHand(handWorldPosition);
    }
    
    void HideImageIfNeeded(string reason)
    {
        if (hideWhenHandNotDetected && imageGameObject.activeSelf)
        {
            if (enableDebugLogs)
                Debug.Log($"HandImagePositioner: {reason} - hiding image");
            imageGameObject.SetActive(false);
        }
        else if (enableDebugLogs && !hideWhenHandNotDetected)
        {
            Debug.Log($"HandImagePositioner: {reason} - but hideWhenHandNotDetected is disabled");
        }
    }
    
    bool IsHandVisibleInCamera(Vector3 handWorldPosition)
    {
        // Convert world position to screen position
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(handWorldPosition);
        
        if (enableDebugLogs)
        {
            Debug.Log($"HandImagePositioner: Visibility check - Screen: {screenPosition}, " +
                     $"Behind camera: {screenPosition.z < 0}, " +
                     $"Outside bounds: {screenPosition.x < 0 || screenPosition.x > UnityEngine.Screen.width || screenPosition.y < 0 || screenPosition.y > UnityEngine.Screen.height}");
        }
        
        // Check if hand is behind camera
        if (hideWhenHandBehindCamera && screenPosition.z < 0)
        {
            if (enableDebugLogs)
                Debug.Log("HandImagePositioner: Hand is behind camera");
            return false;
        }
        
        // Check if hand is outside camera viewport
        if (hideWhenHandOutOfCamera)
        {
            // Check if position is outside screen bounds
            if (screenPosition.x < 0 || screenPosition.x > UnityEngine.Screen.width ||
                screenPosition.y < 0 || screenPosition.y > UnityEngine.Screen.height)
            {
                if (enableDebugLogs)
                    Debug.Log($"HandImagePositioner: Hand is outside camera bounds - X: {screenPosition.x}/{UnityEngine.Screen.width}, Y: {screenPosition.y}/{UnityEngine.Screen.height}");
                return false;
            }
        }
        
        if (enableDebugLogs)
            Debug.Log("HandImagePositioner: Hand is visible in camera");
        
        return true;
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
        
        // Convert screen position to Canvas position
        Vector2 canvasPosition;
        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
        
        if (canvas != null)
        {
            // Handle different canvas render modes
            Camera canvasCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? 
                null : canvas.worldCamera;
                
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPosition,
                canvasCamera,
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
