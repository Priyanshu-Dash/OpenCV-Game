using UnityEngine;

public class InClassGameplayManager : MonoBehaviour
{

    private void Start()
    {
        if (GameSessionManager.selectedPrefab != null)
        {
            GameObject spawned = Instantiate(GameSessionManager.selectedPrefab);
            

            Debug.Log("✅ Game prefab spawned in InClassGames scene.");
        }
        else
        {
            Debug.LogError("❌ No selected prefab to spawn in InClassGames scene!");
        }
    }
}
