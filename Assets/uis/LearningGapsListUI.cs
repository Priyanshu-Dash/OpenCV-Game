using System.Collections.Generic;
using UnityEngine;
using TMPro; // If you're using TextMeshPro
using UnityEngine.UI;

public class LearningGapsListUI : MonoBehaviour
{
    public GameObject textPrefab; // Assign a Text or TextMeshProUGUI prefab in Inspector
    public Transform contentParent; // Assign the Content object of the ScrollView

    private void OnEnable()
    {
        GameCurationManager.OnCurationCompleted += PopulateLearningGaps;
    }

    private void OnDisable()
    {
        GameCurationManager.OnCurationCompleted -= PopulateLearningGaps;
    }

    private void PopulateLearningGaps()
    {
        var curatedGames = FindObjectOfType<GameCurationManager>().curatedGames;

        // Clear old texts if needed
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        HashSet<string> addedGaps = new HashSet<string>();

        foreach (var game in curatedGames)
        {
            foreach (var outcome in game.learningOutcomes)
            {
                if (!addedGaps.Contains(outcome))
                {
                    GameObject newTextObj = Instantiate(textPrefab, contentParent);
                    TMP_Text textComponent = newTextObj.GetComponent<TMP_Text>();

                    if (textComponent != null)
                    {
                        textComponent.text = outcome;
                    }
                    else
                    {
                        Text uiText = newTextObj.GetComponent<Text>();
                        if (uiText != null)
                        {
                            uiText.text = outcome;
                        }
                    }

                    addedGaps.Add(outcome);
                }
            }
        }
    }
}
