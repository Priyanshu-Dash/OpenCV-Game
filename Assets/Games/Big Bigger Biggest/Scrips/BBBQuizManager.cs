using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BBBQuizManager : MonoBehaviour
{
    public Transform leftImage;
    public Transform middleImage;
    public Transform rightImage;

    public TextMeshProUGUI questionText;

    [Header("Result UI")]
    public GameObject resultImage; // Should be initially deactivated
    public Sprite yaySprite;
    public Sprite oopsSprite;
    public AudioClip yayAudio;
    public AudioClip oopsAudio;

    [Header("Game Sprites")]
    public List<Sprite> possibleSprites; // Drag in from Inspector

    private AudioSource resultAudioSource;
    private bool hasAnswered = false;
    private int correctIndex = -1;
    private string[] questionVariants = { "Which one is big?", "Which one is bigger?", "Which one is biggest?" };

    private void Start()
    {
        resultAudioSource = resultImage.GetComponent<AudioSource>();
        GenerateQuestion();
    }

    void GenerateQuestion()
    {
        hasAnswered = false;
        resultImage.SetActive(false);

        questionText.text = questionVariants[Random.Range(0, questionVariants.Length)];

        List<float> scales = GenerateDistinctScales();
        List<Transform> images = new List<Transform> { leftImage, middleImage, rightImage };

        for (int i = 0; i < images.Count; i++)
        {
            float scale = scales[i];
            images[i].localScale = new Vector3(scale, scale, 1f);
        }

        // Assign the same random sprite to all 3
        Sprite randomSprite = possibleSprites[Random.Range(0, possibleSprites.Count)];
        foreach (var img in images)
        {
            img.GetComponent<Image>().sprite = randomSprite;
        }

        // Determine correct index based on question
        List<float> sortedScales = new List<float>(scales);
        sortedScales.Sort();

        if (questionText.text.Contains("big?"))         // Smallest
            correctIndex = scales.IndexOf(sortedScales[0]);
        else if (questionText.text.Contains("bigger?"))  // Medium
            correctIndex = scales.IndexOf(sortedScales[1]);
        else if (questionText.text.Contains("biggest?")) // Largest
            correctIndex = scales.IndexOf(sortedScales[2]);
    }

    List<float> GenerateDistinctScales()
    {
        float minScale = 0.75f;
        float maxScale = 2f;
        float minDiff = 0.25f;

        List<float> validScales = new List<float>();

        while (true)
        {
            float a = Random.Range(minScale, maxScale);
            float b = Random.Range(minScale, maxScale);
            float c = Random.Range(minScale, maxScale);

            List<float> values = new List<float> { a, b, c };
            values.Sort();

            if ((values[1] - values[0] >= minDiff) && (values[2] - values[1] >= minDiff))
            {
                validScales = values;
                break;
            }
        }

        // Shuffle
        for (int i = 0; i < validScales.Count; i++)
        {
            float temp = validScales[i];
            int rand = Random.Range(i, validScales.Count);
            validScales[i] = validScales[rand];
            validScales[rand] = temp;
        }

        return validScales;
    }

    public void SelectLeftOptionFromGesture() => HandleAnswer(0);
    public void SelectMiddleOptionFromGesture() => HandleAnswer(1);
    public void SelectRightOptionFromGesture() => HandleAnswer(2);

    void HandleAnswer(int selectedIndex)
    {
        if (hasAnswered) return;
        hasAnswered = true;

        bool isCorrect = (selectedIndex == correctIndex);
        Debug.Log((isCorrect ? "CORRECT" : "WRONG") + " answer selected");

        StartCoroutine(ShowResultAndNextQuestion(isCorrect));
    }

    IEnumerator ShowResultAndNextQuestion(bool isCorrect)
    {
        resultImage.GetComponent<Image>().sprite = isCorrect ? yaySprite : oopsSprite;
        resultImage.SetActive(true);

        if (resultAudioSource != null)
        {
            resultAudioSource.clip = isCorrect ? yayAudio : oopsAudio;
            resultAudioSource.Play();
        }

        yield return new WaitForSeconds(1.25f);

        resultImage.SetActive(false);
        GenerateQuestion();
    }
}
