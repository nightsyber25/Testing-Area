using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;


    private void Awake()
    {
        instance = this;
    }

    public enum EventCodes : byte
    {
        Setup,
        StartGameDraw,
        SpecialCardPhase,
        NormalCardPhase,
        DetermineWinner,
        Endturn,
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[]data = (object[])photonEvent.CustomData;

            switch(theEvent)
            {
                case EventCodes.Setup:

                SetupRecieve(data);

                break;

            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        if(!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0);
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SetupRecieve(object[] dataRecieve)
    {

    }

    public class PlayerInfo
    {
        public string name;
        public int card1,card2,card3;
        public int countWin,countLose;

        public PlayerInfo(string _name,int _card1,int _card2,int _card3,int _countWin,int _countLose)
        {
            name = _name;
            card1 = _card1;
            card2 = _card2;
            card3 = _card3;
            countWin = _countWin;
            countLose = _countLose;
        }
    }
}
