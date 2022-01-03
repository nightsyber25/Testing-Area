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
    public List<Card> Deck = new List<Card>();
    public int countWinToWin = 3;
    public GameState state = GameState.Setup;
    private int index;

    private void Awake()
    {
        instance = this;
    }

    public enum EventCodes : byte
    {
        InitPlayer,
        PlayerList,
        SetupPhrase,
        DetermineWinner,
        UpdatedPlayerSelectedCard,
        UpdatedScore,
    }

    public enum GameState
    {
        Setup,
        SpecialCard,
        NormalCard,
        EndRound,
        EndMatch
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


                case EventCodes.PlayerList:

                    PlayerListRecieve(data);

                    break;

                case EventCodes.SetupPhrase:

                    SetupPhaseRecieve(data);

                    break; 

                case EventCodes.DetermineWinner:

                    DetermineWinnerRecieve(data);

                    break;

                case EventCodes.UpdatedPlayerSelectedCard:

                    UpdatedPlayerSelectedCardRecieve(data);

                    break;
                case EventCodes.UpdatedScore:


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

        if(PhotonNetwork.IsMasterClient)
        {
            SetupPhaseSend();
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
        PlayerListSend();
    }

    //Shared list to all player
    public void PlayerListSend()
    {
        object[] package = new object[allPlayers.Count];

        for(int i = 0; i < allPlayers.Count; i++)
        {
            object[] piece = new object[4];

            piece[0] = allPlayers[i].name;
            piece[1] = allPlayers[i].actor;
            piece[2] = allPlayers[i].selectedCard;
            piece[3] = allPlayers[i].countWin;

            package[i] = piece;
            
        }

        PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerList,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All},
        new SendOptions { Reliability = true }
        );

    }

    public void PlayerListRecieve(object[] dataRecieve)
    {
        allPlayers.Clear();

        for (int i = 0; i < dataRecieve.Length; i++)
        {
            object[] piece = (object[])dataRecieve[i];
            PlayerInfo player = new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (string)piece[2],
                (int)piece[3]
            );
            allPlayers.Add(player);

            if(PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
            {
                index = i;
            }
        }
    }

    public void SetupPhaseSend()
    {
        object[] package = new object[2];

        package[0] = DeckController.instance.GenerateDeck();

        
    }

    public void SetupPhaseRecieve(object[] dataRecieve)
    {
        
    }

    public void DetermineWinnerSend()
    {
        object[] package = new object[2];
        switch (allPlayers[0].selectedCard)
        {
            case "Scissor":
                switch (allPlayers[1].selectedCard)
                {
                    case "Scissor":
                        package[0] = null;
                        break;
                    case "Paper":
                        package[0] = allPlayers[0].name;
                        break;
                    case "Rock":
                        package[0] = allPlayers[1].name;
                        break;
                }
                break;
            case "Paper":
                switch (allPlayers[1].selectedCard)
                {
                    case "Scissor":
                        package[0] = allPlayers[1].name;
                        break;
                    case "Paper":
                        package[0] = null;
                        break;
                    case "Rock":
                        package[0] = allPlayers[0].name;
                        break;
                }
                break;
            case "Rock":
                switch (allPlayers[1].selectedCard)
                {
                    case "Scissor":
                        package[0] = allPlayers[0].name;
                        break;
                    case "Paper":
                        package[0] = allPlayers[1].name;
                        break;
                    case "Rock":
                        package[0] = null;
                        break;
                }
                break;
        }
        package[1] = GameState.EndRound;

        PhotonNetwork.RaiseEvent((byte)EventCodes.DetermineWinner,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All },
        new SendOptions { Reliability = true }
        );
    }

    public void DetermineWinnerRecieve(object[] dataRecieve)
    { 
        string winner = (string)dataRecieve[0];
        state = (GameState)dataRecieve[1];

        for(int i = 0;i < allPlayers.Count;i++)
        {
            if(allPlayers[i].name == winner)
            {
                allPlayers[i].countWin += 1;
                Debug.Log("Updated "+ allPlayers[i].name + " Count win: "+ allPlayers[i].countWin);
                UIController.instance.statusText.text = allPlayers[i].name + "Win!!!";
                UIController.instance.statusText.gameObject.SetActive(true);
            }
            else
            {
                UIController.instance.statusText.text = "Tie";
                UIController.instance.statusText.gameObject.SetActive(true);
            }
        }
    }

    private void WinCheck()
    {
        bool winnerFound = false;
        foreach(PlayerInfo player in allPlayers)
        {
            if(player.countWin > 3 && player.countWin > 0)
            {
                winnerFound = true;
                break;
            }
        }

        if(winnerFound)
        {
            state = GameState.EndMatch;
        }
    }

    public void UpdatedPlayerSelectedCardSend(int actorSending,string card)
    {
        object[] package = new object[] {actorSending , card};

        PhotonNetwork.RaiseEvent((byte)EventCodes.UpdatedPlayerSelectedCard,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All },
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
