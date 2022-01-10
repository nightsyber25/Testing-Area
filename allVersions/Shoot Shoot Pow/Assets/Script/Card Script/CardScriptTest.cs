using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CardScriptTest : MonoBehaviourPunCallbacks , IClicked
{
    public void OnClick()
    {
        MatchManager.instance.UpdatedPlayerSelectedCardSend(PhotonNetwork.LocalPlayer.ActorNumber,int.Parse(gameObject.name));
    }
}
