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
    public List<Card> deck = new List<Card>();
    public int countWinToWin = 3;
    public GameState state = GameState.Setup;
    private int index;
    public int currentNumberOfCardInDeck = 0;
    public float phaseLength = 10f;
    private float currentPhaseTime;

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
        DrawCard,
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

                    InitPlayerReceive(data);

                    break;


                case EventCodes.PlayerList:

                    PlayerListReceive(data);

                    break;

                case EventCodes.SetupPhrase:

                    SetupPhaseReceive(data);

                    break; 

                case EventCodes.DetermineWinner:

                    DetermineWinnerReceive(data);

                    break;

                case EventCodes.UpdatedPlayerSelectedCard:

                    UpdatedPlayerSelectedCardReceive(data);

                    break;
                case EventCodes.UpdatedScore:


                    break;

                case EventCodes.DrawCard:

                    DrawCardReceive(data);

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
            SetPhaseTimer();
        }

        if(PhotonNetwork.IsMasterClient)
        {
            SetupPhaseSend();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //&& (state == GameState.SpecialCard || state == GameState.NormalCard
        if(currentPhaseTime > 0f )
        {
            currentPhaseTime -= Time.deltaTime;
            UpdateTimerDisplay();
            if(currentPhaseTime <= 0f)
            {
                currentPhaseTime = 0f;
                StateCheck();
            }
        }
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

    public void SetPhaseTimer()
    {
        if(phaseLength > 0)
        {
            currentPhaseTime = phaseLength;
            UpdateTimerDisplay();
        }
    }

    public void UpdateTimerDisplay()
    {
        var timeToDisplay = System.TimeSpan.FromSeconds(currentPhaseTime);
        
        UIController.instance.phaseTimer.text = timeToDisplay.Seconds.ToString("00");
    }

    void StateCheck()
    {
        if(state == GameState.SpecialCard)
        {
            state = GameState.NormalCard;
            SetPhaseTimer();
        }
        else if(state == GameState.NormalCard)
        {
            state = GameState.SpecialCard;
            SetPhaseTimer();
        }
    }

    // ---------- Photon Sending and Receiving Data ----------
    public void InitPlayerSend(string username)
    {
        object[] package = new object[7];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
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

    public void InitPlayerReceive(object[] dataReceive)
    {
        PlayerInfo player = new PlayerInfo((string)dataReceive[0],(int)dataReceive[1],(string)dataReceive[2],(int)dataReceive[3],(int)dataReceive[4],(int)dataReceive[5],(int)dataReceive[6]);

        allPlayers.Add(player);
        PlayerListSend();
    }


    // ---------- Photon Sending and Receiving Data ----------
    //Shared list to all player
    public void PlayerListSend()
    {
        object[] package = new object[allPlayers.Count];

        for(int i = 0; i < allPlayers.Count; i++)
        {
            object[] piece = new object[7];

            piece[0] = allPlayers[i].name;
            piece[1] = allPlayers[i].actor;
            piece[2] = allPlayers[i].selectedCard;
            piece[3] = allPlayers[i].countWin;
            piece[4] = allPlayers[i].inHandCard1;
            piece[5] = allPlayers[i].inHandCard2;
            piece[6] = allPlayers[i].inHandCard3;

            package[i] = piece;
            
        }

        PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerList,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All},
        new SendOptions { Reliability = true }
        );

    }

    
    public void PlayerListReceive(object[] dataReceive)
    {
        allPlayers.Clear();

        for (int i = 0; i < dataReceive.Length; i++)
        {
            object[] piece = (object[])dataReceive[i];
            PlayerInfo player = new PlayerInfo(
                (string)piece[0],
                (int)piece[1],
                (string)piece[2],
                (int)piece[3],
                (int)piece[4],
                (int)piece[5],
                (int)piece[6]
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
        List<Card> deckTemp = DeckController.instance.GenerateDeck();
        currentNumberOfCardInDeck = deckTemp.Count;

        object[] package = new object[currentNumberOfCardInDeck];

        for(int i = 0; i < currentNumberOfCardInDeck; i++)
        {
            object[] piece = new object[1];

            piece[0] = deckTemp[i].cardID;

            package[i] = piece;
        }

        PhotonNetwork.RaiseEvent((byte)EventCodes.SetupPhrase,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All},
        new SendOptions { Reliability = true }
        );
    }

    public void SetupPhaseReceive(object[] dataReceive)
    {
        deck.Clear();
        for(int i = 0; i < dataReceive.Length; i++)
        {
            object[] piece = (object[])dataReceive[i];
            Card CardInDeck = new Card((int)piece[0]);
            deck.Add(CardInDeck);
        }

        UIController.instance.SetSetupScreen();
        state = GameState.SpecialCard;
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

    public void DetermineWinnerReceive(object[] dataReceive)
    { 
        string winner = (string)dataReceive[0];
        state = (GameState)dataReceive[1];

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

    

    public void UpdatedPlayerSelectedCardSend(int actorSending,string card)
    {
        object[] package = new object[] {actorSending ,card};

        PhotonNetwork.RaiseEvent((byte)EventCodes.UpdatedPlayerSelectedCard,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All },
        new SendOptions { Reliability = true }
        );
    }

    public void UpdatedPlayerSelectedCardReceive(object[] dataReceive)
    {
        int actor = (int)dataReceive[0];
        string card = (string)dataReceive[1];

        for(int i = 0;i < allPlayers.Count;i++)
        {
            if(allPlayers[i].actor == actor)
            {
                allPlayers[i].selectedCard = card;
                Debug.Log("Updated "+ allPlayers[i].name + " Card: "+ allPlayers[i].selectedCard);
            }
        }
    }

    public void DrawCardSend(string playerName, int drawAmount)
    {
        if(currentNumberOfCardInDeck > drawAmount)
        {
            object[] package = new object[3];

            package[0] = playerName;

            package[1] = drawAmount;

            PhotonNetwork.RaiseEvent((byte)EventCodes.DrawCard,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
        }
        else
        {
            Debug.Log("Insufficient card\n");
        }
    }
    // ---------- ------------------------ ----------

    public void DrawCardReceive(object[] dataReceive)
    {
         for(int i = 0; i < dataReceive.Length; i++)
        {
            object[] piece = (object[])dataReceive[i];
            Card CardInDeck = new Card((int)piece[0]);
            deck.Add(CardInDeck);
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


    public class PlayerInfo
    {
        public string name;
        public int actor;
        public string selectedCard;
        public int countWin;
        public int inHandCard1 ,inHandCard2 ,inHandCard3;
        

        public PlayerInfo(string _name, int _actor,string _selectedCard,int _countWin,int _inHandCard1,int _inHandCard2,int _inHandCard3)
        {
            name = _name;
            actor = _actor;
            selectedCard = _selectedCard;
            countWin = _countWin;
            inHandCard1 = _inHandCard1;
            inHandCard2 = _inHandCard2;
            inHandCard3 = _inHandCard3;
        }
    }

}
