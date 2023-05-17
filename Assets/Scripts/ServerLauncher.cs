using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class ServerLauncher : MonoBehaviourPunCallbacks
{
    public static ServerLauncher instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject loadingPanel;
    public GameObject menuButtons;

    public TMP_Text loadingText;

    public GameObject createRoomPanel;
    public TMP_InputField roomNameInput;

    public GameObject insideRoomPanel;
    public TMP_Text roomNameText;
    public TMP_Text playerNameLabel;
    private List<TMP_Text> allPlayerNicknames = new List<TMP_Text>();

    public GameObject errorPanel;
    public TMP_Text errorText;

    public GameObject roomBrowserPanel;
    public RoomBrowse theRoomButton;

    public List<RoomBrowse> allRoomButtons = new List<RoomBrowse>();
    private Dictionary<string, RoomInfo> cachedRoomsList = new Dictionary<string, RoomInfo>();

    public GameObject createNicknamePanel;
    public TMP_InputField nicknameInput;
    public static bool hasSetNickname;

    public string levelNameToPlay;
    public GameObject startGameButton;

    public GameObject choosePanel;
    void Start()
    {
        CloseMenus();

        loadingPanel.SetActive(true);
        loadingText.text = "Connecting To Network...";
        
        PhotonNetwork.ConnectUsingSettings();
       
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void CloseMenus()
    {
        loadingPanel.SetActive(false);
        menuButtons.SetActive(false);
        createRoomPanel.SetActive(false);
        insideRoomPanel.SetActive(false);
        errorPanel.SetActive(false);
        roomBrowserPanel.SetActive(false);
        createNicknamePanel.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        loadingText.text = "Joining Lobby...";

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        cachedRoomsList.Clear();
        CloseMenus();
        menuButtons.SetActive(true);

        PhotonNetwork.NickName = Random.Range(0, 1000).ToString();

        if (!hasSetNickname)
        {
            CloseMenus();
            createNicknamePanel.SetActive(true);

            if (PlayerPrefs.HasKey("playerName"))
            {
                nicknameInput.text = PlayerPrefs.GetString("playerName");
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
    }

    public override void OnLeftLobby()
    {
        cachedRoomsList.Clear();
    }

    public void OpenRoomCreate()
    {
        CloseMenus();
        createRoomPanel.SetActive(true);
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            #region comment
            // using Photon.Realtime
            #endregion
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 8;

            PhotonNetwork.CreateRoom(roomNameInput.text, options);

            CloseMenus();
            loadingText.text = "Creating Room...";
            loadingPanel.SetActive(true);
        }
    }

    public override void OnJoinedRoom()
    {
        CloseMenus();
        insideRoomPanel.SetActive(true);

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        ListAllPlayers();

        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Failed to create a room: " + message;
        CloseMenus();
        errorPanel.SetActive(true);
    }

    public void CloseErrorPanel()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        CloseMenus();
        loadingText.text = "Leaving Room";
        loadingPanel.SetActive(true);
    }

    public override void OnLeftRoom()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public void OpenRoomBrowser()
    {
        CloseMenus();
        roomBrowserPanel.SetActive(true);
    }

    public void CloseRoomBrowser()
    {
        CloseMenus();
        menuButtons.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
    }

    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomsList.Remove(info.Name);
            }
            else
            {
                cachedRoomsList[info.Name] = info;
            }
        }
        RoomListButtonUpdate(cachedRoomsList);
    }

    public void RoomListButtonUpdate(Dictionary<string, RoomInfo> cachedRoomList)
    {
        foreach (RoomBrowse rb in allRoomButtons)
        {
            Destroy(rb.gameObject);
        }
        allRoomButtons.Clear();

        theRoomButton.gameObject.SetActive(false);

        foreach (KeyValuePair<string, RoomInfo> roomInfo in cachedRoomList)
        {
            RoomBrowse newButton = Instantiate(theRoomButton, theRoomButton.transform.parent);
            newButton.SetButtonDetails(roomInfo.Value);
            newButton.gameObject.SetActive(true);
            allRoomButtons.Add(newButton);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomsList.Clear();
    }
    public void JoinRoom(RoomInfo inputInfo)
    {
        PhotonNetwork.JoinRoom(inputInfo.Name);
        CloseMenus();
        loadingText.text = "Joining Room";
        loadingPanel.SetActive(true);
    }

    private void ListAllPlayers()
    {
        foreach (TMP_Text playerNickname in allPlayerNicknames)
        {
            Destroy(playerNickname.gameObject);
        }
        allPlayerNicknames.Clear();

        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
            newPlayerLabel.text = players[i].NickName;
            newPlayerLabel.gameObject.SetActive(true);

            allPlayerNicknames.Add(newPlayerLabel);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TMP_Text newPlayerLabel = Instantiate(playerNameLabel, playerNameLabel.transform.parent);
        newPlayerLabel.text = newPlayer.NickName;
        newPlayerLabel.gameObject.SetActive(true);
        allPlayerNicknames.Add(newPlayerLabel);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ListAllPlayers();
    }

    public void SetNickname()
    {
        if (!string.IsNullOrEmpty(nicknameInput.text))
        {
            PhotonNetwork.NickName = nicknameInput.text;

            PlayerPrefs.SetString("playerName", nicknameInput.text);

            CloseMenus();
            menuButtons.SetActive(true);
            hasSetNickname = true;
        }
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(levelNameToPlay);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChooseCharacter()
    {
        choosePanel.SetActive(true);
    }
}
