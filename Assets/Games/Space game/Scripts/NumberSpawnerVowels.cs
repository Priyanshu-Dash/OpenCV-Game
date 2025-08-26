using UnityEngine;
using UnityEngine.UI;

public class NumberSpawnerVowels : MonoBehaviour
{
    public GameObject letterPrefab; // Prefab with 3D text to display letters
    public Transform player; // Reference to the player
    public float spawnDistance = 30f; // Distance ahead of the player to spawn letters
    public float spawnInterval = 2f; // Time between spawns
    public float laneWidth = 3f; // Width of the lanes
    public Text modeinfo;
    private string[] vowels = { "A", "E", "I", "O", "U" }; // List of vowels
    private string[] consonants = { "B", "C", "D", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "V", "W", "X", "Y", "Z" }; // List of consonants

    void Start()
    {
        if (GameManager.Instance.selectedMode == "Vowels")
        {
            modeinfo.text = "Pickup Vowels: A E I O U";
        }
        // Start spawning letters periodically
        InvokeRepeating(nameof(SpawnLetter), 1f, spawnInterval);
    }

    void SpawnLetter()
    {
        int randomLane = Random.Range(-1, 2); // Random lane (-1, 0, 1)
        float xPosition = randomLane * laneWidth; // Calculate x-position based on lane
        Vector3 spawnPosition = new Vector3(xPosition, 1f, player.position.z + spawnDistance);

        // Decide whether to spawn a vowel or a consonant
        bool spawnValid = Random.value > 0.5f; // 50% chance for vowel
        string spawnedLetter = spawnValid ? GenerateVowel() : GenerateConsonant();

        GameObject spawnedLetterObject = Instantiate(letterPrefab, spawnPosition, Quaternion.identity);
        spawnedLetterObject.GetComponent<Number>().SetValue(spawnedLetter);
        spawnedLetterObject.GetComponent<Number>().IsValid = spawnValid;
    }

    string GenerateVowel()
    {
        return vowels[Random.Range(0, vowels.Length)];
    }

    string GenerateConsonant()
    {
        return consonants[Random.Range(0, consonants.Length)];
    }
}
