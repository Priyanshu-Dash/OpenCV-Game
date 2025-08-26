using Mediapipe.Unity;
using UnityEngine;
using Mediapipe.Tasks.Vision.PoseLandmarker;
using Mediapipe.Tasks.Components.Containers;
using Mediapipe.Unity.Sample.PoseLandmarkDetection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class BodyTrackerCE : MonoBehaviour
{
    //public PoseLandmarkerRunner poselandmarkerrunner;
    public PoseLandmarkerRunner poselandmarkerrunner;
    public CartController controller;
    private PoseLandmarks poseLandmarks;
    private float prevRightHipX = 0f;
    private float prevLeftHipX = 0f;
    private const float movementThreshold = 0.02f; // Adjusted for better movement detection
    bool turning = false;
    public float turningtime = 1f;
    float time;
    [System.Serializable]
    public class Landmark
    {
        public float x;
        public float y;
        public float z;
        public float visibility;
        public float presence;
    }

    [System.Serializable]
    public class PoseLandmarks
    {
        public List<Landmark> landmarks;
    }

    // void Start(){
    //     poselandmarkerrunner = FindObjectOfType<LandMarkGenerator>();
    // }

    void Update()
    {
        if(turning){
            time += Time.deltaTime;
            if(time > turningtime){
                time = 0f;
                turning = false;
            }
        }
        string jsonString = poselandmarkerrunner.landmarkdata;
        if (string.IsNullOrEmpty(jsonString))
        {
            Debug.LogWarning("No pose landmark data received.");
            return;
        }

        poseLandmarks = JsonConvert.DeserializeObject<PoseLandmarks>(jsonString);
        if (poseLandmarks == null || poseLandmarks.landmarks == null || poseLandmarks.landmarks.Count < 33)
        {
            Debug.LogError("Invalid pose landmark data.");
            return;
        }

        DetectBasicMovements();
    }

    private void DetectBasicMovements()
    {
        Vector3 leftShoulder = new Vector3(poseLandmarks.landmarks[11].x, poseLandmarks.landmarks[11].y, poseLandmarks.landmarks[11].z);
        Vector3 rightShoulder = new Vector3(poseLandmarks.landmarks[12].x, poseLandmarks.landmarks[12].y, poseLandmarks.landmarks[12].z);
        Vector3 leftHip = new Vector3(poseLandmarks.landmarks[23].x, poseLandmarks.landmarks[23].y, poseLandmarks.landmarks[23].z);
        Vector3 rightHip = new Vector3(poseLandmarks.landmarks[24].x, poseLandmarks.landmarks[24].y, poseLandmarks.landmarks[24].z);
        Vector3 leftAnkle = new Vector3(poseLandmarks.landmarks[27].x, poseLandmarks.landmarks[27].y, poseLandmarks.landmarks[27].z);
        Vector3 rightAnkle = new Vector3(poseLandmarks.landmarks[28].x, poseLandmarks.landmarks[28].y, poseLandmarks.landmarks[28].z);

        // **Jump Detection** - Check if hips are lifted higher
        float avgHipY = (leftHip.y + rightHip.y) / 2;
        bool isJumping = avgHipY < 0.45f && avgHipY < 0.3f; // Match WebGL logic

        // **Running Detection** - Check foot distance
        float footDistance = Mathf.Abs(leftAnkle.y - rightAnkle.y);
        bool isRunning = footDistance > 0.2f;

        // **Leaning Detection** - Compare shoulder and hip positions
        bool leanLeft = leftShoulder.x > leftHip.x + 0.1f;
        bool leanRight = rightShoulder.x < rightHip.x - 0.1f;

        // **Movement Detection** - Compare hip X position over time
        bool moveRight = prevRightHipX != 0 && rightHip.x - prevRightHipX > movementThreshold;
        bool moveLeft = prevRightHipX != 0 && prevRightHipX - rightHip.x > movementThreshold;

        // **Action Detection**
        if (moveRight)
        {
            turning = true;
            time = 0f;
            controller.TurnRight();
        }
        else if (moveLeft)
        {
            turning = true;
            time = 0f;
            controller.TurnLeft();
        }
        else if (isJumping)
        {
            controller.Jump();
        }
        else if (leanLeft)
        {
            //controller.LeanLeft();
            controller.LeanRight();
        }
        else if (leanRight)
        {
            //controller.LeanRight();
            controller.LeanLeft();
        }
        else if (isRunning)
        {
            controller.Run();
        }
        else if(!turning)
        {
            controller.Stand();
        }

        // Update previous positions for movement tracking
        prevRightHipX = rightHip.x;
        prevLeftHipX = leftHip.x;
    }
}