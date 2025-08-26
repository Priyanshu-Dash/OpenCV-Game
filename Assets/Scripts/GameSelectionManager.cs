using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class GameThumbnailEntry
{
    public string gameId;
    public string title;
    public Sprite thumbnail;
    public GameObject prefab;
}

public class GameSelectionManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameEntryPrefab;      // Prefab with thumbnail, title and play button
    public Transform scrollContentParent;   // Content parent in ScrollView
    public float entrySpacing = 20f;        // Space between entries
    public GameObject transitionScreen;     // Loading screen

    [Header("Game Data")]
    public List<GameThumbnailEntry> gameEntries;
    public List<GameMetadata> curatedGames;

    private bool isLoading = false;

    private void OnEnable()
    {
        GameCurationHelper.OnCurationCompleted += InitializeSelectionScreen;
    }

    private void OnDisable()
    {
        GameCurationHelper.OnCurationCompleted -= InitializeSelectionScreen;
    }

    private void InitializeSelectionScreen()
    {
        if (GameCurationHelper.Instance == null)
        {
            Debug.LogWarning("⚠️ GameCurationHelper.Instance not found.");
            return;
        }

        curatedGames = GameCurationHelper.Instance.curatedGames;

        if (curatedGames == null || curatedGames.Count == 0)
        {
            Debug.LogWarning("🚫 No curated games to display in GameSelectionManager.");
            return;
        }

        Debug.Log("✅ Curated games loaded in GameSelectionManager: " + string.Join(", ", curatedGames.ConvertAll(g => g.gameId)));
        
        PopulateScrollView();
    }

    private void PopulateScrollView()
    {
        // Clear existing entries
        foreach (Transform child in scrollContentParent)
        {
            Destroy(child.gameObject);
        }

        if (gameEntryPrefab == null || scrollContentParent == null)
        {
            Debug.LogError("❌ Missing required UI references for scroll view");
            return;
        }

        // Calculate total content height needed
        float totalHeight = 0f;
        RectTransform entryRect = gameEntryPrefab.GetComponent<RectTransform>();
        float entryHeight = entryRect.sizeDelta.y;

        foreach (GameMetadata game in curatedGames)
        {
            // Find matching thumbnail entry
            var entry = gameEntries.Find(e => e.gameId == game.gameId);
            if (entry == null)
            {
                Debug.LogWarning($"⚠️ No thumbnail entry found for game: {game.gameId}");
                continue;
            }

            // Create new entry
            GameObject newEntry = Instantiate(gameEntryPrefab, scrollContentParent);
            newEntry.name = $"Entry_{game.gameId}";

            // Set position
            RectTransform rt = newEntry.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, -totalHeight);
            totalHeight += entryHeight + entrySpacing;

            // Apply random gradient background to the parent image
            ApplyRandomGradientBackground(newEntry);

            // Set UI elements
            Image thumbnail = newEntry.transform.Find("Thumbnail")?.GetComponent<Image>();
            TMP_Text title = newEntry.transform.Find("Title")?.GetComponent<TMP_Text>();
            Button playButton = newEntry.transform.Find("PlayButton")?.GetComponent<Button>();

            if (thumbnail != null) thumbnail.sprite = entry.thumbnail;
            if (title != null) title.text = game.title;

            if (playButton != null)
            {
                playButton.onClick.AddListener(() => PlayGame(entry));
            }
            else
            {
                Debug.LogWarning($"⚠️ Play button missing in entry for: {game.gameId}");
            }
        }

        // Set content height
        RectTransform contentRect = scrollContentParent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
    }

    private void ApplyRandomGradientBackground(GameObject entry)
    {
        Image background = entry.GetComponent<Image>();
        if (background == null)
        {
            Debug.LogWarning("No Image component found on game entry for gradient background");
            return;
        }

        // Generate a fully saturated and bright random color for the top
        Color topColor = Color.white; // Full saturation and brightness
        Color bottomColor = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f);

        // Create a vertical 1x64 texture
        int height = 64;
        Texture2D texture = new Texture2D(1, height);
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / (height - 1);
            Color color = Color.Lerp(topColor, bottomColor, t); // Lerp from topColor to white
            texture.SetPixel(0, y, color);
        }
        texture.Apply();

        // Create a sprite from the texture
        Sprite gradientSprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        background.sprite = gradientSprite;
        background.type = Image.Type.Simple; // Use Simple to preserve gradient integrity
    }

    private void PlayGame(GameThumbnailEntry entry)
    {
        if (isLoading) return;

        if (entry != null && entry.prefab != null)
        {
            GameSession.selectedPrefab = entry.prefab;
            Debug.Log($"▶️ Launching game: {entry.gameId}");

            RecentGamesListUI.instance?.AddRecentGame(entry.gameId);
            StartCoroutine(LoadGameAfterDelay());
        }
        else
        {
            Debug.LogError("❌ Invalid game entry or missing prefab");
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
            Debug.LogWarning("⚠️ No transition screen assigned!");
        }

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("gamemaker");
    }
}