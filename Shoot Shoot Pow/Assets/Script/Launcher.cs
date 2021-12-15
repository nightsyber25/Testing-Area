using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject menuButton;
    [SerializeField] TMP_Text loadingText;

    [SerializeField] GameObject createRoomScreen;
    [SerializeField] TMP_InputField roomNameInput;

    [SerializeField] GameObject roomScreen;
    [SerializeField] TMP_Text roomNameText, playerNameText;
    List<TMP_Text> allPlayerNames = new List<TMP_Text>();

    [SerializeField] GameObject errorScreen;
    [SerializeField] TMP_Text errorText;

    [SerializeField] GameObject roomBrowserScreen;
    [SerializeField] RoomButton theRoomButton;
    List<RoomButton> allRoomButtons = new List<RoomButton>();

    [SerializeField] GameObject nameInputScreen;
    [SerializeField] TMP_InputField nameInput;
    bool hasSetName;

    [SerializeField] string map;
    [SerializeField] GameObject startButton;

    [SerializeField] GameObject roomTestButton;


    private void Awake()
    {
        instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        CloseMenus();

        loadingScreen.SetActive(true);
        loadingText.text = "Connecting to Network...";
        PhotonNetwork.ConnectUsingSettings();

#if UNITY_EDITOR
        roomTestButton.SetActive(true);        
#endif
    }

    private void CloseMenus()
    {
        loadingScreen.SetActive(false);
        menuButton.SetActive(false);
        createRoomScreen.SetActive(false);
        roomScreen.SetActive(false);
        errorScreen.SetActive(false);
        roomBrowserScreen.SetActive(false);
        nameInputScreen.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();

        PhotonNetwork.AutomaticallySyncScene = true;
        
        loadingText.text = "Joining Lobby...";
    }

    public override void OnJoinedLobby()
    {
        CloseMenus();
        menuButton.SetActive(true);

        if(!hasSetName)
        {
            CloseMenus();
            nameInputScreen.SetActive(true);

            if(PlayerPrefs.HasKey("playerName"))
            {
                nameInput.text = PlayerPrefs.GetString("playerName");
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
    }

    public void OpenRoomCreate()
    {
        CloseMenus();
        createRoomScreen.SetActive(true);
    }

    public void CreateRoom()
    {
        if(!string.IsNullOrEmpty(roomNameInput.text))
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 4;

            PhotonNetwork.CreateRoom(roomNameInput.text);

            CloseMenus();
            loadingText.text = "Creating Room...";
            loadingScreen.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        roomScreen.SetActive(true);

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        ListAllPlayers();

        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }

    }

    private void ListAllPlayers()
    {
        foreach(TMP_Text playerName in allPlayerNames)
        {
            Destroy(playerName.gameObject);
        }
        allPlayerNames.Clear();

        Player[] players = PhotonNetwork.PlayerList;
        
        for(int i = 0; i < players.Length; i++)
        {
            TMP_Text newPlayerName = Instantiate(playerNameText,playerNameText.transform.parent);
            newPlayerName.text = players[i].NickName;
            newPlayerName.gameObject.SetActive(true);

            allPlayerNames.Add(newPlayerName);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerName = Instantiate(playerNameText,playerNameText.transform.parent);
            newPlayerName.text = newPlayer.NickName;
            newPlayerName.gameObject.SetActive(true);

            allPlayerNames.Add(newPlayerName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Failed to create room: "+ message;
        CloseMenus();
        errorScreen.SetActive(true);
    }

    public void CloseErrorScreen()
    {
        CloseMenus();
        menuButton.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        loadingText.text = "Leaving Room...";
        loadingScreen.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButton.SetActive(true);
    }

    public void OpenRoomBrowser()
    {
        CloseMenus();
        roomBrowserScreen.SetActive(true);
    }

    public void CloseRoomBrowser()
    {
        CloseMenus();
        menuButton.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomButton rb in allRoomButtons)
        {
            Destroy(rb.gameObject);
        }
        allRoomButtons.Clear();
        
        theRoomButton.gameObject.SetActive(false);

        for(int i = 0;i < roomList.Count;i++)
        {
            if(roomList[i].PlayerCount != roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
            {
                RoomButton newButton = Instantiate(theRoomButton,theRoomButton.transform.parent);
                newButton.SetButtonDetails(roomList[i]);
                newButton.gameObject.SetActive(true);

                allRoomButtons.Add(newButton);
            }
        }
    }

    public void JoinRoom(RoomInfo inputInfo)
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);

        CloseMenus();
        loadingText.text = "Joining Room...";
        loadingScreen.SetActive(true);
    }

    public void SetPlayerName()
    {
        if(!string.IsNullOrEmpty(nameInput.text))
        {
            PhotonNetwork.NickName = nameInput.text;

            PlayerPrefs.SetString("playerName", nameInput.text);

            CloseMenus();
            menuButton.SetActive(true);
            hasSetName = true;
        }
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(map);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if(PhotonNetwork.IsMasterClient)
        {
            startButton.SetActive(true);
        }
        else
        {
            startButton.SetActive(false);
        }
    }

    public void QuickJoin()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        
        PhotonNetwork.CreateRoom("Test");
        CloseMenus();
        loadingText.text = "Creating Test Room...";
        loadingScreen.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    




}
