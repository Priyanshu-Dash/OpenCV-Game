using System.Collections.Generic;
using UnityEngine;

public class GamePrefabDatabase : MonoBehaviour
{
    public List<GamePrefabEntry> prefabEntries;

    public static GamePrefabDatabase Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetPrefabByGameId(string gameId)
    {
        foreach (var entry in prefabEntries)
        {
            if (entry.gameId == gameId)
                return entry.prefab;
        }

        Debug.LogWarning($"❌ No prefab found for gameId: {gameId}");
        return null;
    }
}
