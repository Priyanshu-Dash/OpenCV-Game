using UnityEngine;
using TMPro;
using DG.Tweening;

public class AnswerBall : MonoBehaviour
{
    public TextMeshPro textMesh;
    public string answerText;
    public float fallDuration = 4f; 
    public bool hasEnded = false;

    private BallQuizManager questionManager;

    public void Initialize(string answer, BallQuizManager manager)
    {
        answerText = answer;
        questionManager = manager;
        textMesh.text = answer;

        StartFalling();
    }

    void StartFalling()
    {
        float endY = -6f;
        transform.DOMoveY(endY, fallDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (!hasEnded)
                {
                    hasEnded = true;
                    questionManager.OnAnswerBallMissed();
                    Destroy(gameObject);
                }
            });
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Basket") && !hasEnded)
        {
            hasEnded = true;
            questionManager.OnAnswerBallCaught(answerText);
            Destroy(gameObject);
        }
    }
}
