using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public PlayerController player;

    public void ReceiveInput(string action)
    {
        // Call appropriate methods in PlayerController
        switch (action)
        {
            case "Jump":
                player.Jump();
                break;
            case "TurnLeft":
                player.TurnLeft();
                break;
            case "TurnRight":
                player.TurnRight();
                break;
            case "LeanLeft":
                player.LeanLeft();
                break;
            case "LeanRight":
                player.LeanRight();
                break;
            case "Running":
                player.Run();
                break;
            case "Standing":
                player.Stand();
                break;
        }
    }
}