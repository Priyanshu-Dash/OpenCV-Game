using UnityEngine;
using UnityEngine.UI;

public class NumberSpawnerPAC : MonoBehaviour
{
    public GameObject wordPrefab; // Prefab for spawning words
    public Transform player; // Reference to the player
    public float spawnDistance = 30f; // Distance ahead of the player to spawn words
    public float spawnInterval = 2f; // Time between spawns
    public float laneWidth = 3f; // Width of the lanes
    public Text questionText; // UI element for the question

    private string[] prepositions = {
        "in", "on", "at", "by", "with", "about", "to", "from", "over", "under", 
        "through", "against", "within", "without", "toward", "beneath", "beside", 
        "behind", "between", "among", "along", "around", "inside", "outside", 
        "near", "across", "beyond", "before", "after", "upon", "during"
    };

    private string[] conjunctions = {
        "and", "or", "but", "so", "because", "although", "if", "when", "while", 
        "since", "though", "unless", "as", "where", "than", "whether", "yet", 
        "nor", "either", "neither", "both", "until", "once", "whereas", "after", 
        "before", "however", "meanwhile", "therefore", "hence", "nonetheless"
    };

    private bool isPrepositionQuestion = true; // Determines if the current question is about prepositions or conjunctions

    private int spawnCount = 0; // Tracks the number of spawns
    private int changeQuestionAfterSpawns = 8; // Change question every 10 spawns

    void Start()
    {
        // Start spawning words periodically
        InvokeRepeating(nameof(SpawnWord), 1f, spawnInterval);

        // Generate the first question
        GenerateQuestion();
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
            GenerateQuestion();
        }
    }

    void GenerateQuestion()
    {
        // Alternate between preposition and conjunction questions
        isPrepositionQuestion = !isPrepositionQuestion;

        // Update the UI text
        questionText.text = isPrepositionQuestion ? "Pick up Prepositions!" : "Pick up Conjunctions!";
    }

    string GenerateValidWord()
    {
        if (isPrepositionQuestion)
        {
            // Valid word for preposition question
            return prepositions[Random.Range(0, prepositions.Length)];
        }
        else
        {
            // Valid word for conjunction question
            return conjunctions[Random.Range(0, conjunctions.Length)];
        }
    }

    string GenerateInvalidWord()
    {
        if (isPrepositionQuestion)
        {
            // Invalid word for preposition question (use conjunctions as invalid)
            return conjunctions[Random.Range(0, conjunctions.Length)];
        }
        else
        {
            // Invalid word for conjunction question (use prepositions as invalid)
            return prepositions[Random.Range(0, prepositions.Length)];
        }
    }
}
