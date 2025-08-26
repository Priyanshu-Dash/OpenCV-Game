using UnityEngine;
using UnityEngine.UI;

public class NumberSpawner2 : MonoBehaviour
{
    public GameObject numberPrefab; // Assign a prefab with a 3D number
    public Transform player; // Reference to the player object
    public float spawnDistance = 30f; // Distance ahead of the player to spawn numbers
    public float spawnInterval = 2f; // Time between spawns
    public float laneWidth = 3f; // Width of the lanes
    public Text questionText; // UI element for the question
    private float nextSpawnZ = 0f; // Z-position of the next spawn

    private int correctAnswer; // Holds the correct answer for the current question
    private int num1, num2; // Numbers for the question
    private string operation; // "+" or "-"

    private int spawnCount = 0; // Tracks the number of spawns
    private int changeQuestionAfterSpawns = 8; // Change question every 10 spawns

    void Start()
    {
        // Start spawning numbers periodically

        InvokeRepeating(nameof(SpawnNumber), 1f, spawnInterval);

        // Generate the first question
        if (GameManager.Instance.selectedMode == "AS")
            GenerateQuestionAS();
        else
        {
            GenerateQuestionMD();
        }
    }

    void SpawnNumber()
    {
        int randomLane = Random.Range(-1, 2); // Random lane (-1, 0, 1)
        float xPosition = randomLane * laneWidth; // Calculate x-position based on lane
        Vector3 spawnPosition = new Vector3(xPosition, 1f, player.position.z + spawnDistance);

        // Decide whether to spawn the correct or incorrect answer
        bool spawnValid = Random.value > 0.5f; // 50% chance for valid answer
        int spawnedNumber = spawnValid ? correctAnswer : GenerateInvalidAnswer();

        GameObject spawnedNumberObject = Instantiate(numberPrefab, spawnPosition, Quaternion.identity);
        spawnedNumberObject.GetComponent<Number>().SetValue(spawnedNumber);
        spawnedNumberObject.GetComponent<Number>().IsValid = spawnValid;
        spawnCount++;

        // Periodically change the question
        if (spawnCount % changeQuestionAfterSpawns == 0)
        {
            if (GameManager.Instance.selectedMode == "AS")
                GenerateQuestionAS();
            else
            {
                GenerateQuestionMD();
            }
        }
    }

void GenerateQuestionAS()
{
    // Randomly select addition or subtraction
    operation = Random.value > 0.5f ? "+" : "-";

    // Generate random numbers for the question
    num1 = Random.Range(1, 50);
    num2 = Random.Range(1, 50);

    // Calculate the correct answer
    correctAnswer = operation == "+" ? num1 + num2 : num1 - num2;

    // Update the question UI
    questionText.text = $"{num1} {operation} {num2} = ?";
}

void GenerateQuestionMD()
{
    // Randomly select multiplication or division
    operation = Random.value > 0.5f ? "×" : "÷";

    if (operation == "×")
    {
        // Generate random numbers for multiplication
        num1 = Random.Range(1, 12);
        num2 = Random.Range(1, 12);
        correctAnswer = num1 * num2;
    }
    else
    {
        // Generate random numbers for division
        num2 = Random.Range(1, 12);
        correctAnswer = Random.Range(1, 12); // Random quotient
        num1 = correctAnswer * num2; // Ensure divisible numbers
    }

    // Update the question UI
    questionText.text = $"{num1} {operation} {num2} = ?";
}

int GenerateInvalidAnswer()
{
    int invalidAnswer;
    do
    {
        // Generate a random offset (-10 to +10, excluding 0)
        int offset = Random.Range(-10, 11);
        if (offset == 0) offset = 1; // Ensure offset is not zero
        invalidAnswer = correctAnswer + offset;
    }
    while (invalidAnswer == correctAnswer); // Ensure the answer is invalid

    return invalidAnswer;
}
}
