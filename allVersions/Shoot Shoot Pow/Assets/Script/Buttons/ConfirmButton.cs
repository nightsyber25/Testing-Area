using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConfirmButton : MonoBehaviourPunCallbacks , IClicked
{
    public void OnClick()
    {

        // if(MatchManager.instance.allPlayers[0].selectedCard != null && MatchManager.instance.allPlayers[1].selectedCard != null)
        // {
        //     MatchManager.instance.DetermineWinnerSend();
        // }
        // else
        // {
        //     UIController.instance.statusText.text = "Wait for other player";
        //     UIController.instance.statusText.gameObject.SetActive(true);
        // }
        

        // if(MatchManager.instance.state == MatchManager.GameState.SpecialCard)
        // {
            
        // }
        // else if(MatchManager.instance.state == MatchManager.GameState.NormalCard)
        // {

        // }

        UIController.instance.SetupTimer();
    }
}
