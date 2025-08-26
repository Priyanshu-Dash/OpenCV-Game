using System.Collections.Generic;
using UnityEngine;

public static class GameMetadataLoader
{
    public static List<GameMetadata> LoadAllGameMetadata()
    {
        List<GameMetadata> allGames = new List<GameMetadata>();
        TextAsset[] files = Resources.LoadAll<TextAsset>("GameMetadata");

        foreach (var file in files)
        {
            Debug.Log("📄 Loading game metadata: " + file.name);
            GameMetadata gm = JsonUtility.FromJson<GameMetadata>(file.text);
            allGames.Add(gm);
        }

        return allGames;
    }

}
