using UnityEngine;
using UnityEngine.UI;

public class NumberSpawnerVerbs : MonoBehaviour
{
    public GameObject numberPrefab; // Prefab for spawning words (as GameObjects)
    public Transform player; // Reference to the player object
    public float spawnDistance = 30f; // Distance ahead of the player to spawn words
    public float spawnInterval = 2f; // Time between spawns
    public float laneWidth = 3f; // Width of the lanes
    public Text questionText; // UI element to display the question
    private string correctAnswer; // Holds the correct answer for the current question
    private string currentVerb; // The current verb being tested
    private string questionType; // Question type (e.g., "past tense", "past participle")

    // List of verbs and their forms
    private readonly string[,] verbs = new string[,]
  {
    { "run", "ran", "run" },
    { "eat", "ate", "eaten" },
    { "go", "went", "gone" },
    { "take", "took", "taken" },
    { "write", "wrote", "written" },
    { "drive", "drove", "driven" },
    { "fly", "flew", "flown" },
    { "sing", "sang", "sung" },
    { "begin", "began", "begun" },
    { "swim", "swam", "swum" },
    { "see", "saw", "seen" },
    { "give", "gave", "given" },
    { "choose", "chose", "chosen" },
    { "break", "broke", "broken" },
    { "speak", "spoke", "spoken" },
    { "ride", "rode", "ridden" },
    { "forget", "forgot", "forgotten" },
    { "steal", "stole", "stolen" },
    { "fall", "fell", "fallen" },
    { "wake", "woke", "woken" },
    { "blow", "blew", "blown" },
    { "grow", "grew", "grown" },
    { "throw", "threw", "thrown" },
    { "drink", "drank", "drunk" },
    { "hide", "hid", "hidden" },
    { "tear", "tore", "torn" },
    { "shake", "shook", "shaken" },
    { "freeze", "froze", "frozen" },
    { "drive", "drove", "driven" },
    { "forget", "forgot", "forgotten" }
  };
    private int spawnCount = 0; // Tracks the number of spawns
    private int changeQuestionAfterSpawns = 10; // Change question every 10 spawns

    void Start()
    {
        // Start spawning words periodically
        // if (GameManager.Instance.selectedMode == "Verbs")
        InvokeRepeating(nameof(SpawnWord), 1f, spawnInterval);

        // Generate the first question
        GenerateQuestion();
    }

    void SpawnWord()
    {
        int randomLane = Random.Range(-1, 2); // Random lane (-1, 0, 1)
        float xPosition = randomLane * laneWidth; // Calculate x-position based on lane
        Vector3 spawnPosition = new Vector3(xPosition, 1f, player.position.z + spawnDistance);

        // Decide whether to spawn the correct or incorrect answer
        bool spawnValid = Random.value > 0.5f; // 50% chance for valid answer
        string spawnedWord = spawnValid ? correctAnswer : GenerateInvalidAnswer();

        GameObject spawnedWordObject = Instantiate(numberPrefab, spawnPosition, Quaternion.identity);
        spawnedWordObject.GetComponent<Number>().SetValue(spawnedWord); // Replace Number with your script for displaying words
        spawnedWordObject.GetComponent<Number>().IsValid = spawnValid;

        spawnCount++;

        // Periodically change the question
        if (spawnCount % changeQuestionAfterSpawns == 0)
        {
            GenerateQuestion();
        }
    }

    void GenerateQuestion()
    {
        // Randomly select a verb and a question type
        int randomVerbIndex = Random.Range(0, verbs.GetLength(0));
        currentVerb = verbs[randomVerbIndex, 0];
        questionType = Random.value > 0.5f ? "past tense" : "past participle";

        // Determine the correct answer based on question type
        correctAnswer = questionType == "past tense"
            ? verbs[randomVerbIndex, 1]
            : verbs[randomVerbIndex, 2];

        // Update the question UI
        questionText.text = $"What is the {questionType} of '{currentVerb}'?";
    }

    string GenerateInvalidAnswer()
    {
        string invalidAnswer;
        do
        {
            // Randomly pick a wrong verb form from the list
            int randomVerbIndex = Random.Range(0, verbs.GetLength(0));
            invalidAnswer = questionType == "past tense"
                ? verbs[randomVerbIndex, 1]
                : verbs[randomVerbIndex, 2];
        }
        while (invalidAnswer == correctAnswer); // Ensure it's not the correct answer

        return invalidAnswer;
    }
}
