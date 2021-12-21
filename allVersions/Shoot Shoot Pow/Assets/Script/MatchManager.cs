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
    public List<PlayerInfo> allPlayers = new List<PlayerInfo>();

    private void Awake()
    {
        instance = this;
    }

    public enum EventCodes : byte
    {
        Setup,
        InitPlayer,
        StartGameDraw,
        SpecialCardPhase,
        NormalCardPhase,
        DetermineWinner,
        Endturn,
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;
            object[] data = (object[])photonEvent.CustomData;

            Debug.Log("Event: " + theEvent);

            switch (theEvent)
            {
                case EventCodes.Setup:

                    SetupRecieve(data);

                    break;

                case EventCodes.InitPlayer:

                    InitPlayerRecieve(data);

                    break;

            }
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            InitPlayerSend(PhotonNetwork.NickName);
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

    public void InitPlayerSend(string username)
    {
        object[] package = new object[7];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0;
        package[3] = 0;
        package[4] = 0;
        package[5] = 0;
        package[6] = 0;

        PhotonNetwork.RaiseEvent((byte)EventCodes.InitPlayer,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
        new SendOptions { Reliability = true }
        );
    }

    public void InitPlayerRecieve(object[] dataRecieve)
    {
        PlayerInfo player = new PlayerInfo((string)dataRecieve[0],(int)dataRecieve[1],(int)dataRecieve[2],(int)dataRecieve[3],(int)dataRecieve[4],(int)dataRecieve[5],(int)dataRecieve[6]);

        allPlayers.Add(player);
    }

    public class PlayerInfo
    {
        public string name;
        public int actor;
        public int card1, card2, card3;
        public int countWin, countLose;

        public PlayerInfo(string _name, int _actor, int _card1, int _card2, int _card3, int _countWin, int _countLose)
        {
            name = _name;
            actor = _actor;
            card1 = _card1;
            card2 = _card2;
            card3 = _card3;
            countWin = _countWin;
            countLose = _countLose;
        }
    }
}
