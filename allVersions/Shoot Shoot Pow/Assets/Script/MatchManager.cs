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
    [SerializeField] int countWinToWin = 5;
    public GameState state = GameState.Setup;
    private int index;
    public int currentNumberOfCardInDeck = 0;
    [SerializeField] float phaseLength = 60f;
    private float currentPhaseTime;

    private void Awake()
    {
        instance = this;
    }

    public enum GameState
    {
        Setup,
        SpecialCard,
        NormalCard,
        EndRound,
        EndMatch
    }

    // ---------- Define photon event ----------
    public enum EventCodes : byte
    {
        InitPlayer,
        PlayerList,
        SetupPhrase,
        DetermineWinner,
        UpdatedPlayerSelectedCard,
        DrawCard,
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

        if (PhotonNetwork.IsMasterClient)
        {
            SetupPhaseSend();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentPhaseTime > 0f && (state == GameState.SpecialCard || state == GameState.NormalCard))
        {
            currentPhaseTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if((allPlayers[0].selectedCard != 0 && allPlayers[1].selectedCard !=0) && currentPhaseTime > 3)
            {
                currentPhaseTime = 3f;
            }

            if (currentPhaseTime <= 0f)
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

    // ---------- Match Timer ----------
    public void SetPhaseTimer()
    {
        if (phaseLength > 0)
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

    // ---------- State ----------
    void StateCheck()
    {
        if (state == GameState.SpecialCard)
        {
            state = GameState.NormalCard;
            SetPhaseTimer();
        }
        else if (state == GameState.NormalCard)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                DetermineWinnerSend();
            }
        }
        else if (state == GameState.EndRound)
        {
            WinCheck();
        }
    }

    private void WinCheck()
    {
        bool winnerFound = false;
        foreach (PlayerInfo player in allPlayers)
        {
            if(player.countWin >= countWinToWin)
            {
                winnerFound = true;
                break;
            }
        }

        if (winnerFound)
        {
            state = GameState.EndMatch;
            EndGame();
        }
        else
        {
            StartCoroutine(EndRo());
        }
    }

    private void EndGame()
    {
        state = GameState.EndMatch;

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(EndCo());
    }

    private IEnumerator EndCo()
    {
        yield return new WaitForSeconds(5f);
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }

    private IEnumerator EndRo()
    {
        yield return new WaitForSeconds(3f);
        foreach(PlayerInfo player in allPlayers)
        {
            player.selectedCard = 0;
        }
        UIController.instance.statusText.gameObject.SetActive(false);
        state = GameState.NormalCard;
        SetPhaseTimer();
    }

    // ---------- Photon Sending and Receiving Event ----------
    public void InitPlayerSend(string username)
    {
        object[] package = new object[2];
        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;

        PhotonNetwork.RaiseEvent((byte)EventCodes.InitPlayer,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
        new SendOptions { Reliability = true }
        );
    }

    public void InitPlayerReceive(object[] dataReceive)
    {
        PlayerInfo player = new PlayerInfo((string)dataReceive[0], (int)dataReceive[1]);

        allPlayers.Add(player);
        PlayerListSend();
    }

    public void PlayerListSend()
    {
        object[] package = new object[allPlayers.Count];

        for (int i = 0; i < allPlayers.Count; i++)
        {
            object[] piece = new object[2];

            piece[0] = allPlayers[i].username;
            piece[1] = allPlayers[i].actor;

            package[i] = piece;

        }

        PhotonNetwork.RaiseEvent((byte)EventCodes.PlayerList,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All },
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
                (int)piece[1]
            );
            allPlayers.Add(player);

            if (PhotonNetwork.LocalPlayer.ActorNumber == player.actor)
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

        for (int i = 0; i < currentNumberOfCardInDeck; i++)
        {
            object[] piece = new object[1];

            piece[0] = deckTemp[i].cardID;

            package[i] = piece;
        }

        PhotonNetwork.RaiseEvent((byte)EventCodes.SetupPhrase,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All },
        new SendOptions { Reliability = true }
        );
    }

    public void SetupPhaseReceive(object[] dataReceive)
    {
        deck.Clear();
        for (int i = 0; i < dataReceive.Length; i++)
        {
            object[] piece = (object[])dataReceive[i];
            Card CardInDeck = new Card((int)piece[0]);
            deck.Add(CardInDeck);
        }

        UIController.instance.SetSetupScreen();
    }

    public void DetermineWinnerSend()
    {
        object[] package = new object[2];
        switch (allPlayers[0].selectedCard)
        {
            case 1:
                switch (allPlayers[1].selectedCard)
                {
                    case 1:
                        package[0] = null;
                        break;
                    case 2:
                        package[0] = allPlayers[0].username;
                        break;
                    case 3:
                        package[0] = allPlayers[1].username;
                        break;
                }
                break;
            case 2:
                switch (allPlayers[1].selectedCard)
                {
                    case 1:
                        package[0] = allPlayers[1].username;
                        break;
                    case 2:
                        package[0] = null;
                        break;
                    case 3:
                        package[0] = allPlayers[0].username;
                        break;
                }
                break;
            case 3:
                switch (allPlayers[1].selectedCard)
                {
                    case 1:
                        package[0] = allPlayers[0].username;
                        break;
                    case 2:
                        package[0] = allPlayers[1].username;
                        break;
                    case 3:
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

        

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].username == winner)
            {
                allPlayers[i].countWin += 1;
                Debug.Log("Updated " + allPlayers[i].username + " Count win: " + allPlayers[i].countWin);
                UIController.instance.statusText.text = allPlayers[i].username + "Win!!!";
                UIController.instance.statusText.gameObject.SetActive(true);
                break;
            }
            else
            {
                UIController.instance.statusText.text = "Tie";
                UIController.instance.statusText.gameObject.SetActive(true);
            }
        }


        StateCheck();

    }

    public void UpdatedPlayerSelectedCardSend(int actorSending, int card)
    {
        object[] package = new object[] { actorSending, card };

        PhotonNetwork.RaiseEvent((byte)EventCodes.UpdatedPlayerSelectedCard,
        package,
        new RaiseEventOptions { Receivers = ReceiverGroup.All },
        new SendOptions { Reliability = true }
        );
    }

    public void UpdatedPlayerSelectedCardReceive(object[] dataReceive)
    {
        int actor = (int)dataReceive[0];
        int card = (int)dataReceive[1];

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].actor == actor)
            {
                allPlayers[i].selectedCard = card;
                Debug.Log("Updated " + allPlayers[i].username + " Card: " + allPlayers[i].selectedCard);
            }
        }
    }

    public void DrawCardSend(string playerName, int drawAmount)
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

    public void DrawCardReceive(object[] dataReceive)
    {
        string playerName = (string)dataReceive[0];
        int drawAmount = (int)dataReceive[1];

        int i = 0;
        int emptyHandSlot = 0;
        

        for (i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].username == playerName)
            {
                if ((allPlayers[i].inHandCardCount + drawAmount) > 3)
                {
                    Debug.Log("Number of cards exceed the hand capacity\n");
                    break;
                }
                else if(currentNumberOfCardInDeck >= drawAmount)
                {
                    while(drawAmount > 0)
                    {
                        emptyHandSlot = PlayerInfo.checkInhandCard(allPlayers[i]);
                        switch(emptyHandSlot)
                        {
                            case 1:
                                allPlayers[i].inHandCard1 = deck[currentNumberOfCardInDeck - 1].cardID;
                                break;
                            case 2:
                                allPlayers[i].inHandCard2 = deck[currentNumberOfCardInDeck - 1].cardID;
                                break;
                            case 3:
                                allPlayers[i].inHandCard3 = deck[currentNumberOfCardInDeck - 1].cardID;
                                break;
                            default:
                                Debug.Log("Error obtaining card\n");
                                break;
                        }

                        drawAmount--;
                        currentNumberOfCardInDeck--;
                    }
                }
                else
                    {
                        Debug.Log("Insufficient card to draw\n");
                        break;
                    }
            }
        }

        
        
    }

    // public class PlayerInfo
    // {
    //     public string name;
    //     public int actor;
    //     public int selectedCard;
    //     public int countWin;
    //     public int inHandCard1, inHandCard2, inHandCard3, inHandCardCount;


    //     public PlayerInfo(string _name, int _actor)
    //     {
    //         name = _name;
    //         actor = _actor;
    //         countWin = 0;
    //         selectedCard = 0;
    //         inHandCard1 = 0;
    //         inHandCard2 = 0;
    //         inHandCard3 = 0;
    //         inHandCardCount = 0;
    //     }

    //     public int checkInhandCard()
    //     {
    //         if(inHandCard1 == 0)
    //         {
    //             return 1;
    //         }
    //         else if (inHandCard2 == 0)
    //         {
    //             return 2;
    //         }
    //         else if(inHandCard3 == 0)
    //         {
    //             return 3;
    //         }
    //         else
    //         {
    //             return 0;
    //         }
    //     }
    // }
}
