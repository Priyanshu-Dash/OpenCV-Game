using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    private void Start()
    {
        if (GameSession.selectedPrefab != null)
        {
            Instantiate(GameSession.selectedPrefab);
        }
        else
        {
            Debug.LogError("❌ No selected prefab to spawn in Gameplay Scene!");
        }
    }
}
