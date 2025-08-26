using UnityEngine;

public class ObstacleGenerator : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public GameObject[] obstacles; // Array of obstacle prefabs
    public float spawnInterval = 2f; // Time interval between spawns
    public float spawnDistance = 20f; // Distance from the player to spawn obstacles
    public float spawnOffsetX = 5f; // Maximum horizontal offset for obstacle spawning

    [Header("Player Reference")]
    public Transform player; // Reference to the player's transform

    private float nextSpawnTime;

    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        // Check if it's time to spawn a new obstacle
        if (Time.time >= nextSpawnTime)
        {
            SpawnObstacle();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnObstacle()
    {
        // Choose a random obstacle prefab
        GameObject obstaclePrefab = obstacles[Random.Range(0, obstacles.Length)];

        // Calculate spawn position
        Vector3 spawnPosition = player.position + Vector3.forward * spawnDistance;
        spawnPosition.x += Random.Range(-spawnOffsetX, spawnOffsetX);

        // Instantiate the obstacle
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
}
