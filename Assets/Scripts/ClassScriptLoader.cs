using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ClassScriptLoader : MonoBehaviour
{
    public GameObject thumbnailPrefab; // UI prefab with Image, Text, and Button
    public Transform scrollContent;    // Scroll view content object
    public List<GameThumbnailListing> gameThumbnails; // Manually assigned in Inspector

    private void Start()
    {
        LoadAndDisplayGames();
    }

    void LoadAndDisplayGames()
    {
        List<string> curatedIds = DeepLinkManager.CuratedGameIDs;

        if (curatedIds == null || curatedIds.Count == 0)
        {
            Debug.LogWarning("No curated game IDs received via deep link.");
            return;
        }

        // Load all game metadata
        List<GameMetadata> allGames = GameMetadataLoader.LoadAllGameMetadata();

        // Filter based on IDs from deep link
        var matchedGames = allGames.Where(game => curatedIds.Contains(game.gameId)).ToList();

        Debug.Log("Displaying Curated Games: " + string.Join(", ", matchedGames.Select(g => g.title)));

        foreach (var game in matchedGames)
        {
            GameThumbnailListing entry = gameThumbnails.Find(e => e.gameId == game.gameId);
            if (entry == null) continue;

            GameObject item = Instantiate(thumbnailPrefab, scrollContent);
            item.transform.Find("Image").GetComponent<Image>().sprite = entry.thumbnail;
            item.transform.Find("title").GetComponent<TMPro.TMP_Text>().text = game.title;

            Button playButton = item.transform.Find("PlayButton").GetComponent<Button>();
            playButton.onClick.AddListener(() =>
            {
                GameSessionManager.selectedPrefab = entry.prefab;
                UnityEngine.SceneManagement.SceneManager.LoadScene("InClassGames");
            });
        }
    }
}
