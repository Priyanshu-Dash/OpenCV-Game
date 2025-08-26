using System.Collections.Generic;
using UnityEngine;

public class GameCurationHelper : MonoBehaviour
{
    public static GameCurationHelper Instance;
    public List<GameMetadata> curatedGames = new List<GameMetadata>();

    public delegate void CurationEvent();
    public static event CurationEvent OnCurationCompleted;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadCuratedGames();
    }

    void LoadCuratedGames()
    {
        List<string> curatedIds = DeepLinkManager.CuratedGameIDs;
        if (curatedIds == null || curatedIds.Count == 0)
        {
            Debug.LogWarning("📭 No curated game IDs received for at-home.");
            return;
        }

        var allGames = GameMetadataLoader.LoadAllGameMetadata();
        curatedGames = allGames.FindAll(g => curatedIds.Contains(g.gameId));

        Debug.Log("📦 Curated At-Home Games Loaded: " + string.Join(", ", curatedGames.ConvertAll(g => g.gameId)));

        OnCurationCompleted?.Invoke();
    }
}
