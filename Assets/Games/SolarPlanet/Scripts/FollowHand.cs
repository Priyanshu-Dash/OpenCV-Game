using UnityEngine;
using UnityEngine.UI;
using Mediapipe.Unity;

public class FollowHand : MonoBehaviour
{
    public delegate void MediPipeClicked(Vector3 postion, Transform _PointObject);
    public MediPipeClicked ClickEnter, ClickStay, ClickExit;

    [Header("Tracking Setup")]
    [SerializeField] private MultiHandLandmarkListAnnotation hands; // Reference to landmark list
    [SerializeField] private Camera mainCamera;

    [Header("Player Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Text boxNameTxt;

    [Header("Tracking Settings")]
    [SerializeField] private int landmarkIndex = 0; // Index Hand
    [SerializeField] private float moveSpeed = 10f; // Follow speed


    private int pointFingerTop = 8;
    private int thumbTop = 4;
    private bool buttonClicked;

    private GameObject selectObject;
    private void Update()
    {
        
        if (hands == null || hands.count == 0 || hands[0] == null)
            return;

        // Get landmark point for the first hand
        var point = hands[0][landmarkIndex];

        if (point == null || !point.gameObject.activeSelf)
            return;

        // Convert world to screen and back to world to align with camera
        Vector3 screenPos = mainCamera.WorldToScreenPoint(point.transform.position);
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
        worldPos.z = transform.position.z; // Maintain original Z (2D)

        // Smooth movement toward hand landmark
        transform.position = Vector3.Lerp(transform.position, worldPos, Time.deltaTime * moveSpeed);

        var point1 = hands[0][pointFingerTop];

        if (point1 == null || !point1.gameObject.activeSelf)
        {
            return;
        }
        else
        {
           // buttonClicked = false;
        }
           
        var point2 = hands[0][thumbTop];

        if (point2 == null || !point2.gameObject.activeSelf)
        {
            return;
        }
        else
        {
           // buttonClicked = false;
        }

        if (Vector3.Distance(point1.transform.position, point2.transform.position) < 5)
        {
            if(buttonClicked == false)
            {
                buttonClicked = true;
               
                ClickEnter?.Invoke(point1.transform.position, point1.transform);
                // first time cliecked
            }
            
        }
        else if(Vector3.Distance(point1.transform.position, point2.transform.position) > 10)
        {
            
            if (buttonClicked)
            {
                buttonClicked = false;
                ClickExit?.Invoke(point1.transform.position, point1.transform);
                selectObject = null;
                //  clieck Exit
            }
        }

        if (buttonClicked)
        {

        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            SpriteRenderer other = collision.GetComponent<SpriteRenderer>();
            if (other != null)
            {
                // Change color and name based on collided object
                spriteRenderer.color = other.color;
                boxNameTxt.text = other.name;
            }
        }
    }
}
