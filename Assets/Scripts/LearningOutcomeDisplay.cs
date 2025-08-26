using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LearningOutcomeDisplay : MonoBehaviour
{
    [Header("References")]
    public GameSelectionManager gameSelectionManager;  // Drag the GameSelectionManager here
    public Transform outcomesContentPanel;             // Scroll View → Content
    public GameObject outcomeTextPrefab;               // Text prefab to instantiate

    private int lastIndex = -1;

    void Update()
    {
        // Check if current index changed
        if (gameSelectionManager == null || gameSelectionManager.curatedGames == null || gameSelectionManager.curatedGames.Count == 0)
            return;

        int currentIndex = GetCurrentIndex();

        if (currentIndex != lastIndex)
        {
            lastIndex = currentIndex;
            DisplayOutcomes(gameSelectionManager.curatedGames[currentIndex]);
        }
    }

    private int GetCurrentIndex()
    {
        // Access the internal index through reflection (since it's private)
        var indexField = typeof(GameSelectionManager).GetField("currentIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int)indexField.GetValue(gameSelectionManager);
    }

    private void DisplayOutcomes(GameMetadata metadata)
    {
        // Clear old list
        foreach (Transform child in outcomesContentPanel)
        {
            Destroy(child.gameObject);
        }

        // Create entries for each outcome
        foreach (string outcome in metadata.learningOutcomes)
        {
            GameObject entry = Instantiate(outcomeTextPrefab, outcomesContentPanel);
            entry.GetComponent<TMP_Text>().text = outcome;
        }
    }
}
