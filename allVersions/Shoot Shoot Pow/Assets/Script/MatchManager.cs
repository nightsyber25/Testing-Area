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
        InitPlayer,
        DetermineWinner,
        UpdatedPlayerSelectedCard,
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
                case EventCodes.InitPlayer:

                    InitPlayerRecieve(data);

                    break;

                case EventCodes.DetermineWinner:

                    InitPlayerRecieve(data);

                    break;

                case EventCodes.UpdatedPlayerSelectedCard:

                    UpdatedPlayerSelectedCardRecieve(data);

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

    public void InitPlayerSend(string username)
    {
        object[] package = new object[4];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = "";
        package[3] = 0;

        PhotonNetwork.RaiseEvent((byte)EventCodes.InitPlayer,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
        new SendOptions { Reliability = true }
        );
    }

    public void InitPlayerRecieve(object[] dataRecieve)
    {
        PlayerInfo player = new PlayerInfo((string)dataRecieve[0],(int)dataRecieve[1],(string)dataRecieve[2],(int)dataRecieve[3]);

        allPlayers.Add(player);
    }

    public void DetermineWinnerSend()
    {
        string card1,card2;
        card1 = allPlayers[0].selectedCard;
        card2 = allPlayers[1].selectedCard;
        GameplayController.instance.DetermineWinner(card1,card2);
    }

    public void DetermineWinnerRecieve()
    {

    }

    public void UpdatedPlayerSelectedCardSend(int actorSending,string card)
    {
        object[] package = new object[] {actorSending , card};

        PhotonNetwork.RaiseEvent((byte)EventCodes.UpdatedPlayerSelectedCard,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
        new SendOptions { Reliability = true }
        );

    }

    public void UpdatedPlayerSelectedCardRecieve(object[] dataRecieve)
    {
        int actor = (int)dataRecieve[0];
        string card = (string)dataRecieve[1];

        for(int i = 0;i < allPlayers.Count;i++)
        {
            if(allPlayers[i].actor == actor)
            {
                allPlayers[i].selectedCard = card;
                Debug.Log("Updated "+ allPlayers[i].name + " Card: "+ allPlayers[i].selectedCard);
            }
        }
    }

    public class PlayerInfo
    {
        public string name;
        public int actor;
        public string selectedCard;
        public int countWin;

        public PlayerInfo(string _name, int _actor,string _selectedCard,int _countWin)
        {
            name = _name;
            actor = _actor;

            selectedCard = _selectedCard;
            countWin = _countWin;
        }
    }
}
