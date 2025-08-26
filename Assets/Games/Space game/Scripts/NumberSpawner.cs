using UnityEngine;
using UnityEngine.UI;
public class NumberSpawner : MonoBehaviour
{
    public GameObject numberPrefab; // Assign a prefab with a 3D number
    public Transform player; // Reference to the player object
    public float spawnDistance = 30f; // Distance ahead of the player to spawn numbers
    public float spawnInterval = 2f; // Time between spawns
    public float laneWidth = 3f; // Width of the lanes
    private float nextSpawnZ = 0f; // Z-position of the next spawn
    public Text modeinfo;

    void Start()
    {
        // Start spawning numbers periodically
        InvokeRepeating(nameof(SpawnNumber), 1f, spawnInterval);

        if(GameManager.Instance.selectedMode == "OddNumbers"){
            modeinfo.text = "Pickup Odd Numbers";
        }
        else if(GameManager.Instance.selectedMode == "EvenNumbers"){
            modeinfo.text = "Pickup Even Numbers";
        }
        else if(GameManager.Instance.selectedMode == "PrimeNumbers"){
            modeinfo.text = "Pickup Prime Numbers";
        }
    }

    void SpawnNumber()
    {
        int randomLane = Random.Range(-1, 2); // Random lane (-1, 0, 1)
        float xPosition = randomLane * laneWidth; // Calculate x-position based on lane
        Vector3 spawnPosition = new Vector3(xPosition, 1f, player.position.z + spawnDistance);

        // Decide whether to spawn a valid or invalid number
        bool spawnValid = Random.value > 0.5f; // 50% chance to spawn a valid number

        int spawnedNumber = spawnValid ? GenerateValidNumber() : GenerateInvalidNumber();

        GameObject spawnedNumberObject = Instantiate(numberPrefab, spawnPosition, Quaternion.identity);
        spawnedNumberObject.GetComponent<Number>().SetValue(spawnedNumber);
        spawnedNumberObject.GetComponent<Number>().IsValid = spawnValid; // Set whether this number is valid
    }

    int GenerateValidNumber()
    {
        switch (GameManager.Instance.selectedMode)
        {
            case "EvenNumbers":
                return GenerateEvenNumber();

            case "OddNumbers":
                return GenerateOddNumber();

            case "PrimeNumbers":
                return GeneratePrimeNumber();

            default:
                return Random.Range(1, 100); // Default valid number
        }
    }

    int GenerateInvalidNumber()
    {
        int number;
        do
        {
            number = Random.Range(1, 100);
        } 
        // Keep generating numbers until it doesn't match the valid logic
        while (IsValidForMode(number));

        return number;
    }

    bool IsValidForMode(int number)
    {
        switch (GameManager.Instance.selectedMode)
        {
            case "EvenNumbers":
                return number % 2 == 0;

            case "OddNumbers":
                return number % 2 != 0;

            case "PrimeNumbers":
                return IsPrime(number);

            default:
                return false;
        }
    }

    int GenerateEvenNumber()
    {
        return Random.Range(1, 50) * 2; // Generate an even number
    }

    int GenerateOddNumber()
    {
        return Random.Range(1, 50) * 2 + 1; // Generate an odd number
    }

    int GeneratePrimeNumber()
    {
        int[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97 };
        return primes[Random.Range(0, primes.Length)]; // Randomly select a prime number
    }

    bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i * i <= number; i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }
}