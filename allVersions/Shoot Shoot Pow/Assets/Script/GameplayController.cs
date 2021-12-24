using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Choices
{
    None,
    Scissor,
    Paper,
    Rock
}

public class GameplayController : MonoBehaviour
{
    // private Choices playerOne = Choices.None, playerTwo = Choices.None;

    public void DetermineWinner(string player1, string player2)
    {
        switch (player1)
        {
            case "Scissor":
                switch (player2)
                {
                    case "Scissor":
                        Debug.Log("Tie");
                        break;
                    case "Paper":
                        Debug.Log("player1: win");
                        Debug.Log("player2: lose");
                        break;
                    case "Rock":
                        Debug.Log("player1: lose");
                        Debug.Log("player2: win");
                        break;
                }
                break;
            case "Paper":
                switch (player2)
                {
                    case "Scissor":
                        Debug.Log("player1: lose");
                        Debug.Log("player2: win");
                        break;
                    case "Paper":
                        Debug.Log("Tie");
                        break;
                    case "Rock":
                        Debug.Log("player1: win");
                        Debug.Log("player2: lose");
                        break;
                }
                break;
            case "Rock":
                switch (player2)
                {
                    case "Scissor":
                        Debug.Log("player1: win");
                        Debug.Log("player2: lose");
                        break;
                    case "Paper":
                        Debug.Log("player1: lose");
                        Debug.Log("player2: win");
                        break;
                    case "Rock":
                        Debug.Log("Tie");
                        break;
                }
                break;
        }
    }
}
