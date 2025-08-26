using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecentGamesListUI : MonoBehaviour
{
    public static RecentGamesListUI instance;

    [Header("Setup")]
    public Transform contentParent; // 👈 Assign the Horizontal Scroll View Content here
    public GameObject imagePrefab;  // 👈 Assign your Image prefab here
    public List<GameThumbnailEntry> allGameEntries; // 👈 Link from GameSelectionManager (or it will auto find)

    private List<GameObject> recentGameImages = new List<GameObject>();
    private List<RecentGameData> recentGamesData = new List<RecentGameData>();

    private const string RecentGamesKey = "RecentGamesList";

    [System.Serializable]
    private class RecentGameData
    {
        public string gameId;
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<RecentGameData> games = new List<RecentGameData>();
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Try auto find game entries if not assigned
        if (allGameEntries == null || allGameEntries.Count == 0)
        {
            var gsm = FindObjectOfType<GameSelectionManager>();
            if (gsm != null)
            {
                allGameEntries = gsm.gameEntries;
            }
        }
    }

    private void Start()
    {
        LoadRecentGames();
    }

    public void AddRecentGame(string gameId)
    {
        // Check if already exists and remove (we'll re-add to top)
        recentGamesData.RemoveAll(g => g.gameId == gameId);

        // Insert at top
        recentGamesData.Insert(0, new RecentGameData { gameId = gameId });

        // Limit to 10
        if (recentGamesData.Count > 10)
            recentGamesData.RemoveAt(recentGamesData.Count - 1);

        SaveRecentGames();
        RefreshUI();
    }

    private void SaveRecentGames()
    {
        var wrapper = new Wrapper { games = recentGamesData };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(RecentGamesKey, json);
        PlayerPrefs.Save();
    }

    private void LoadRecentGames()
    {
        if (PlayerPrefs.HasKey(RecentGamesKey))
        {
            string json = PlayerPrefs.GetString(RecentGamesKey);
            var wrapper = JsonUtility.FromJson<Wrapper>(json);

            if (wrapper != null && wrapper.games != null)
            {
                recentGamesData = wrapper.games;
            }
            else
            {
                recentGamesData = new List<RecentGameData>();
            }

            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        // Clear old
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        recentGameImages.Clear();

        // Recreate
        foreach (var gameData in recentGamesData)
        {
            var entry = allGameEntries.Find(e => e.gameId == gameData.gameId);
            if (entry != null)
            {
                CreateRecentGameEntry(entry.thumbnail);
            }
            else
            {
                Debug.LogWarning($"⚠️ No entry found for GameId: {gameData.gameId}");
            }
        }
    }

    private void CreateRecentGameEntry(Sprite thumbnail)
    {
        GameObject newImageObj = Instantiate(imagePrefab, contentParent);
        Image img = newImageObj.GetComponent<Image>();
        if (img != null)
        {
            img.sprite = thumbnail;
        }
        recentGameImages.Add(newImageObj);
    }
}
