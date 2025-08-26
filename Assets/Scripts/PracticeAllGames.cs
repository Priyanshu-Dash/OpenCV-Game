using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PracticeAllGames : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject gameEntryPrefab;         // Prefab containing thumbnail, title and play button
    public Transform scrollContentParent;      // Content object under ScrollView
    public GameObject transitionScreen;        // Optional loading screen
    public float spacingBetweenEntries = 20f;  // Space between game entries

    [Header("Game Entries")]
    public List<GameThumbnailEntry> allGames;  // List of all available games

    private bool isLoading = false;

    void Start()
    {
        GameSelectionManager manager = FindObjectOfType<GameSelectionManager>();

        if (manager != null)
        {
            allGames = manager.gameEntries;
        }
        else
        {
            Debug.LogWarning("⚠️ GameSelectionManager not found in scene. Using fallback.");
        }

        if (gameEntryPrefab == null || scrollContentParent == null)
        {
            Debug.LogError("❌ Game entry prefab or scroll content parent not assigned.");
            return;
        }

        if (allGames == null || allGames.Count == 0)
        {
            Debug.LogWarning("⚠️ No games found in allGames list.");
            return;
        }

        PopulateGameEntries();
    }

    void PopulateGameEntries()
    {
        // Clear existing entries (if any)
        foreach (Transform child in scrollContentParent)
        {
            Destroy(child.gameObject);
        }

        // Calculate total content height needed
        float totalHeight = 0f;
        RectTransform entryPrefabRect = gameEntryPrefab.GetComponent<RectTransform>();
        float entryHeight = entryPrefabRect.sizeDelta.y;

        foreach (var game in allGames)
        {
            GameObject entryObj = Instantiate(gameEntryPrefab, scrollContentParent);
            entryObj.name = "GameEntry_" + game.gameId;

            // Set position with spacing
            RectTransform entryRect = entryObj.GetComponent<RectTransform>();
            entryRect.anchoredPosition = new Vector2(0, -totalHeight);
            totalHeight += entryHeight + spacingBetweenEntries;

            // 🎨 Set random background color (solid, not gradient)
            Transform backgroundTransform = entryObj.transform.Find("Background");
            if (backgroundTransform != null)
            {
                Image backgroundImage = backgroundTransform.GetComponent<Image>();
                if (backgroundImage != null)
                {
                    Color randomColor = Random.ColorHSV(0f, 1f, 0.6f, 0.9f, 0.7f, 0.9f); // Fully bright and saturated
                    backgroundImage.color = randomColor;
                }
                else
                {
                    Debug.LogWarning($"⚠️ Image component missing on Background for {game.gameId}");
                }
            }
            else
            {
                Debug.LogWarning($"⚠️ Background child not found in prefab for {game.gameId}");
            }

            // Set game data
            Image thumbnailImg = entryObj.transform.Find("Thumbnail")?.GetComponent<Image>();
            TMP_Text titleText = entryObj.transform.Find("Title")?.GetComponent<TMP_Text>();
            Button playButton = entryObj.transform.Find("PlayButton")?.GetComponent<Button>();

            if (thumbnailImg != null) thumbnailImg.sprite = game.thumbnail;
            if (titleText != null) titleText.text = game.title;

            if (playButton != null)
            {
                var capturedGame = game;
                playButton.onClick.AddListener(() => LaunchGame(capturedGame));
            }
            else
            {
                Debug.LogWarning($"⚠️ Play button not found in game entry for: {game.gameId}");
            }
        }
        // Set content height to fit all entries
        RectTransform contentRect = scrollContentParent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
    }

    void LaunchGame(GameThumbnailEntry gameEntry)
    {
        if (isLoading) return;

        if (gameEntry != null && gameEntry.prefab != null)
        {
            GameSession.selectedPrefab = gameEntry.prefab;
            Debug.Log($"▶️ Launching game: {gameEntry.gameId}");
            StartCoroutine(LoadGameAfterDelay());
        }
        else
        {
            Debug.LogError("❌ Missing prefab for game: " + gameEntry?.gameId);
        }
    }

    private IEnumerator LoadGameAfterDelay()
    {
        isLoading = true;

        if (transitionScreen != null)
        {
            transitionScreen.SetActive(true);
        }
        else
        {
            Debug.Log("⏳ Transition screen not assigned. Proceeding directly.");
        }

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("gamemaker");
    }
}