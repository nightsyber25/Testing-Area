using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConfirmButton : MonoBehaviourPunCallbacks, IClicked
{
    public void OnClick(int selectedCard)
    {
        if (MatchManager.instance.state == MatchManager.GameState.SpecialCard)
        {

        }
        else if (MatchManager.instance.state == MatchManager.GameState.NormalCard)
        {
            // if ((MatchManager.instance.allPlayers[0].selectedCard != 0 && MatchManager.instance.allPlayers[1].selectedCard != 0))
            // {
            //     MatchManager.instance.DetermineWinnerSend();
            // }
            // else
            // {
            //     UIController.instance.statusText.text = "Wait for other player";
            //     UIController.instance.statusText.gameObject.SetActive(true);
            // }
            MatchManager.instance.UpdatedPlayerSelectedCardSend(PhotonNetwork.LocalPlayer.ActorNumber,selectedCard);
        }

    }
}
