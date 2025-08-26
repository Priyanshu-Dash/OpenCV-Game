using UnityEngine;
using UnityEngine.UI;

public class SentenceFormation : MonoBehaviour
{
    public GameObject wordPrefab; // Prefab for spawning words
    public Transform player; // Reference to the player
    public float spawnDistance = 30f; // Distance ahead of the player to spawn words
    public float spawnInterval = 2f; // Time between spawns
    public float laneWidth = 3f; // Width of the lanes
    public Text questionText; // UI element for the sentence/question

    private string[] sentences = {
        "The cat ___ on the mat.",
        "She ___ a beautiful painting.",
        "They ___ to the park every weekend.",
        "He ___ his homework yesterday.",
        "We ___ going to the beach tomorrow.",
        "The sun ___ in the east.",
        "She ___ a cake yesterday.",
        "They ___ to the concert last night.",
        "The dog ___ loudly at strangers.",
        "We ___ going to the movies tomorrow.",
        "He ___ his homework before dinner.",
        "I ___ a new book last week.",
        "Birds ___ in the sky.",
        "She ___ her best friend every weekend.",
        "The children ___ in the park.",
        "He ___ to the teacher's instructions carefully.",
        "We ___ a beautiful sunset at the beach.",
        "The cat ___ under the table.",
        "She ___ an email to her manager.",
        "They ___ football every Sunday.",
        "I ___ my breakfast at 7 a.m. every day.",
        "The baby ___ when he is hungry.",
        "He ___ his car to work every day.",
        "The flowers ___ beautiful in spring.",
        "We ___ a movie together last night.",
        "The train ___ at 5 p.m. sharp.",
        "She ___ her bag on the table.",
        "The kids ___ to the music at the party.",
        "He ___ a letter to his grandmother.",
        "We ___ at the top of the hill.",
        "The students ___ for the test next week.",
        "The bird ___ its wings and flew away.",
        "They ___ their car with soap and water.",
        "She ___ to the sound of the alarm clock.",
        "The fire ___ brightly in the night.",
    };

    private string[][] correctOptions = {
        new string[] { "sat" },
        new string[] { "painted" },
        new string[] { "go" },
        new string[] { "finished" },
        new string[] { "are" },
        new string[] { "rises" },
        new string[] { "baked" },
        new string[] { "went" },
        new string[] { "barks" },
        new string[] { "are" },
        new string[] { "finished" },
        new string[] { "bought" },
        new string[] { "fly" },
        new string[] { "visits" },
        new string[] { "played" },
        new string[] { "listens" },
        new string[] { "saw" },
        new string[] { "sleeps" },
        new string[] { "wrote" },
        new string[] { "play" },
        new string[] { "eat" },
        new string[] { "cries" },
        new string[] { "drives" },
        new string[] { "bloom" },
        new string[] { "watched" },
        new string[] { "departs" },
        new string[] { "placed" },
        new string[] { "danced" },
        new string[] { "wrote" },
        new string[] { "stopped" },
        new string[] { "study" },
        new string[] { "spread" },
        new string[] { "cleaned" },
        new string[] { "woke" },
        new string[] { "burned" }
    };

    private string[][] invalidOptions = {
        new string[] { "flies", "eats", "writes" },
        new string[] { "sang", "danced", "played" },
        new string[] { "swim", "jump", "run" },
        new string[] { "ran", "sang", "cried" },
        new string[] { "was", "ran", "ate" },
        new string[] { "sets", "jumps", "sleeps" },
        new string[] { "paints", "runs", "drives" },
        new string[] { "eat", "swims", "plays" },
        new string[] { "sings", "dances", "jumps" },
        new string[] { "were", "will", "went" },
        new string[] { "starts", "forgets", "cooks" },
        new string[] { "reads", "throws", "eats" },
        new string[] { "swim", "crawl", "sleep" },
        new string[] { "forgets", "runs", "drives" },
        new string[] { "studies", "watches", "sleeps" },
        new string[] { "talks", "ignores", "runs" },
        new string[] { "eats", "jumps", "cries" },
        new string[] { "drives", "dances", "flies" },
        new string[] { "reads", "sings", "flies" },
        new string[] { "watches", "dances", "sleeps" },
        new string[] { "flies", "swims", "writes" },
        new string[] { "laughs", "dances", "jumps" },
        new string[] { "rides", "jumps", "sings" },
        new string[] { "sleeps", "cries", "dances" },
        new string[] { "flies", "sings", "eats" },
        new string[] { "arrives", "runs", "sleeps" },
        new string[] { "throws", "eats", "sleeps" },
        new string[] { "swims", "sleeps", "cries" },
        new string[] { "eats", "jumps", "drives" },
        new string[] { "jumps", "cries", "swims" },
        new string[] { "swims", "sleeps", "dances" },
        new string[] { "throws", "eats", "cries" },
        new string[] { "eats", "jumps", "sleeps" },
        new string[] { "sleeps", "cries", "dances" },
        new string[] { "swims", "sleeps", "jumps" }
    };

    private int currentSentenceIndex;
    private int spawnCount = 0; // Tracks the number of spawns
    private int changeQuestionAfterSpawns = 10; // Change question every 10 spawns

    void Start()
    {
        // Start spawning words periodically
        InvokeRepeating(nameof(SpawnWord), 1f, spawnInterval);

        // Generate the first sentence
        GenerateSentence();
    }

    void SpawnWord()
    {
        int randomLane = Random.Range(-1, 2); // Random lane (-1, 0, 1)
        float xPosition = randomLane * laneWidth; // Calculate x-position based on lane
        Vector3 spawnPosition = new Vector3(xPosition, 1f, player.position.z + spawnDistance);

        // Decide whether to spawn a valid or invalid word
        bool spawnValid = Random.value > 0.5f; // 50% chance for valid word
        string spawnedWord = spawnValid ? GenerateValidWord() : GenerateInvalidWord();

        GameObject spawnedWordObject = Instantiate(wordPrefab, spawnPosition, Quaternion.identity);
        spawnedWordObject.GetComponent<Number>().SetValue(spawnedWord);
        spawnedWordObject.GetComponent<Number>().IsValid = spawnValid;

        spawnCount++;

        // Periodically change the question
        if (spawnCount % changeQuestionAfterSpawns == 0)
        {
            GenerateSentence();
        }
    }

    void GenerateSentence()
    {
        // Select the next sentence in sequence (cyclical)
        currentSentenceIndex = Random.Range(0,sentences.Length);

        // Update the question UI
        questionText.text = sentences[currentSentenceIndex];
    }

    string GenerateValidWord()
    {
        // Get a valid word for the current sentence
        return correctOptions[currentSentenceIndex][0]; // Always the first and only correct word
    }

    string GenerateInvalidWord()
    {
        // Get a random invalid word for the current sentence
        string invalidWord;
        do
        {
            invalidWord = invalidOptions[currentSentenceIndex][Random.Range(0, invalidOptions[currentSentenceIndex].Length)];
        }
        while (invalidWord == correctOptions[currentSentenceIndex][0]); // Ensure it's not accidentally valid

        return invalidWord;
    }
}
