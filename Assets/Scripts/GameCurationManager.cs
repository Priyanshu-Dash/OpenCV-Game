using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameCurationManager : MonoBehaviour
{
    public List<GameMetadata> curatedGames;
    public static event Action OnCurationCompleted; // ✅ New event

    private void Start()
    {
        var allGames = GameMetadataLoader.LoadAllGameMetadata();
        var profile = StudentProfileManager.Instance.GetStudentProfile();

        curatedGames = new List<GameMetadata>();

        foreach (var game in allGames)
        {
            foreach (var gap in profile.learningGaps)
            {
                string gapLower = gap.ToLower();

                bool outcomeMatch = game.learningOutcomes.Any(outcome =>
                    gapLower.Contains(outcome.ToLower()) || outcome.ToLower().Contains(gapLower));

                bool keywordMatch = game.keywords != null && game.keywords.Any(k =>
                    gapLower.Contains(k.ToLower()));

                if (outcomeMatch || keywordMatch)
                {
                    curatedGames.Add(game);
                    Debug.Log($"✅ Matched game: {game.title} for gap: {gap}");
                    break;
                }
            }
        }

        if (curatedGames.Count == 0)
        {
            Debug.LogWarning("🚫 No matching games curated. Check outcome keywords and keywords list.");
        }
        else
        {
            Debug.Log($"🎯 {curatedGames.Count} games curated for {profile.studentName}.");
            foreach (var game in curatedGames)
            {
                Debug.Log($"🕹️ Game: {game.title} | Outcomes: {string.Join(", ", game.learningOutcomes)}");
            }
        }

        // ✅ Invoke event after curation is done
        OnCurationCompleted?.Invoke();
    }

    public GameMetadata GetNextGame()
    {
        return curatedGames.Count > 0 ? curatedGames[0] : null;
    }
}
