using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GameRegistry", menuName = "Game/GameRegistry")]
public class GameRegistry : ScriptableObject
{
    public List<GamePrefabInfo> gameList;

    public GameObject GetGameById(string id)
    {
        return gameList.FirstOrDefault(g => g.gameId == id)?.gamePrefab;
    }
}
