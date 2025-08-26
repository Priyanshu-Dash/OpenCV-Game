using UnityEngine;

public class AnimalQuizTrigger : MonoBehaviour
{
    public AnimalRegionQuizManager quizManager;

    private void OnTriggerEnter(Collider other)
    {
        if (quizManager == null) return;

        if (other.CompareTag("LeftWrist"))
        {
            quizManager.SelectLeftOptionFromGesture();
            Debug.Log("selected left");
        }
        else if (other.CompareTag("RightWrist"))
        {
            quizManager.SelectRightOptionFromGesture();
            Debug.Log("selected right");
        }
    }
}
