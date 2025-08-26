using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class CatchEggQuestion
{
    public string questionText;
    public List<string> questionFields;
    public List<string> options;
    public string correctAnswer;
}

public class CatchEggsManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI questionText;
    public List<TextMeshProUGUI> questionSlots;
    public TextMeshProUGUI questionCounterText;

    [Header("Gameplay")]
    public GameObject eggPrefab;
    public Transform[] spawnPoints;
    public Transform dropArea;
    public GameObject cartObject;

    [Header("Egg Appearance")]
    public List<Sprite> eggSprites;

    [Header("Result Feedback")]
    public Image resultImage;
    public Sprite yeySprite;
    public Sprite oopsSprite;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip correctClip;
    public AudioClip wrongClip;

    private List<CatchEggQuestion> questions = new();
    private int currentQuestionIndex = 0;
    private int correctAnswers = 0;
    private bool isAnswering = false;
    private const int totalQuestions = 10;

    void Start()
    {
        LoadQuestions();
        ShowQuestion();
    }

    void LoadQuestions()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Catch Eggs");
        if (jsonFile != null)
        {
            questions = JsonUtilityWrapper.FromJsonList<CatchEggQuestion>(jsonFile.text);
            ShuffleList(questions);
        }
        else
        {
            Debug.LogError("❌ Could not load Catch Eggs.json from Resources.");
        }
    }

    void ShowQuestion()
    {
        if (currentQuestionIndex >= totalQuestions || currentQuestionIndex >= questions.Count)
        {
            EndGame();
            return;
        }

        CatchEggQuestion q = questions[currentQuestionIndex];

        questionText.text = q.questionText;
        questionCounterText.text = $"{currentQuestionIndex + 1}/{totalQuestions}";

        for (int i = 0; i < questionSlots.Count; i++)
        {
            questionSlots[i].text = q.questionFields[i];
        }

        StartCoroutine(SpawnEggOptions(q.options, q.correctAnswer));
    }

    IEnumerator SpawnEggOptions(List<string> options, string correctAnswer)
    {
        isAnswering = true;

        List<int> spawnIndices = new List<int> { 0, 1, 2 };
        ShuffleList(spawnIndices);

        List<Sprite> shuffledSprites = new List<Sprite>(eggSprites);
        ShuffleList(shuffledSprites);

        for (int i = 0; i < options.Count; i++)
        {
            int spawnIndex = spawnIndices[i];
            Transform spawnPoint = spawnPoints[spawnIndex];

            GameObject egg = Instantiate(eggPrefab, spawnPoint.position, Quaternion.identity, dropArea);
            egg.GetComponentInChildren<TextMeshProUGUI>().text = options[i];
            egg.name = "Egg_" + options[i];

            Image eggImage = egg.GetComponent<Image>();
            if (eggImage != null && i < shuffledSprites.Count)
            {
                eggImage.sprite = shuffledSprites[i];
            }

            EggOptionTag tag = egg.AddComponent<EggOptionTag>();
            tag.value = options[i];
            tag.isCorrect = (options[i] == correctAnswer);

            yield return new WaitForSeconds(Random.Range(0.5f, 1f));
        }
    }

    public void OnEggCaught(string value, bool isCorrect)
    {
        if (!isAnswering) return;

        isAnswering = false;

        if (resultImage != null)
        {
            resultImage.gameObject.SetActive(true);
            resultImage.sprite = isCorrect ? yeySprite : oopsSprite;
        }

        if (isCorrect)
        {
            correctAnswers++;
            audioSource?.PlayOneShot(correctClip);
        }
        else
        {
            audioSource?.PlayOneShot(wrongClip);
        }

        StartCoroutine(NextQuestionRoutine());
    }

    IEnumerator NextQuestionRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        resultImage?.gameObject.SetActive(false);

        foreach (Transform child in dropArea)
        {
            Destroy(child.gameObject);
        }

        currentQuestionIndex++;
        ShowQuestion();
    }

    void EndGame()
    {
        Debug.Log("🎮 Game Over");
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}
