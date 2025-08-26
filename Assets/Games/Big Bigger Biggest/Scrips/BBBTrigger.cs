using UnityEngine;

public class BBBTrigger : MonoBehaviour
{
    public BBBQuizManager quizManager;

    private float leftHandStayTime = 0f;
    private float rightHandStayTime = 0f;

    private bool leftInside = false;
    private bool rightInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftWrist"))
        {
            leftInside = true;
        }
        else if (other.CompareTag("RightWrist"))
        {
            rightInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LeftWrist"))
        {
            leftInside = false;
            leftHandStayTime = 0f;
        }
        else if (other.CompareTag("RightWrist"))
        {
            rightInside = false;
            rightHandStayTime = 0f;
        }
    }

    void Update()
    {
        if (leftInside) leftHandStayTime += Time.deltaTime;
        if (rightInside) rightHandStayTime += Time.deltaTime;

        if (leftHandStayTime > 0.25f && rightHandStayTime > 0.25f)
        {
            quizManager.SelectMiddleOptionFromGesture();
            ResetState();
        }
        else if (leftHandStayTime > 0.25f && !rightInside)
        {
            quizManager.SelectLeftOptionFromGesture();
            ResetState();
        }
        else if (rightHandStayTime > 0.25f && !leftInside)
        {
            quizManager.SelectRightOptionFromGesture();
            ResetState();
        }
    }

    void ResetState()
    {
        leftHandStayTime = 0f;
        rightHandStayTime = 0f;
        leftInside = false;
        rightInside = false;
    }
}
