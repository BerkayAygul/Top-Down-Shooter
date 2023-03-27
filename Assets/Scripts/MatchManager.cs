using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;

    public List<PlayerInformation> allPlayersList = new List<PlayerInformation>();

    public Dictionary< int,Inventory> inventories = new Dictionary<int,Inventory>(); //players inventories.

    public GameObject itemTextPrefab;
    public GameObject itemTextPanel;

    private int index;

    private List<LeaderboardPlayerInformation> leaderboardPlayerInformationList = new List<LeaderboardPlayerInformation>();

    public enum EventCodes : byte
    {
        NewPlayerEvent,
        ListPlayersEvent,
        UpdateStatsEvent
    }

    public enum GameStates : byte
    {
        GameWaitingState,
        GamePlayingState,
        GameEndingState
    }

    public GameStates currentGameState = GameStates.GameWaitingState;

    public float waitGameStateTime = 8f;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
        if(!PhotonNetwork.IsConnected)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        else
        {
            NewPlayerEventSend(PhotonNetwork.LocalPlayer.NickName);

            currentGameState = GameStates.GamePlayingState;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && currentGameState != GameStates.GameEndingState)
        {
            if (UIController.instance.leaderboardTableDisplay.activeInHierarchy)
            {
                UIController.instance.leaderboardTableDisplay.SetActive(false);
            }
            else
            {
                ShowLeaderboard();
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code < 200)
        {
            EventCodes theEvent = (EventCodes)photonEvent.Code;

            object[] receivedData = (object[])photonEvent.CustomData;

            switch (theEvent)
            {
                case EventCodes.NewPlayerEvent:
                    NewPlayerEventReceive(receivedData);
                    break;
                case EventCodes.ListPlayersEvent:
                    ListPlayerEventReceive(receivedData);
                    break;
                case EventCodes.UpdateStatsEvent:
                    UpdateStatsEventReceive(receivedData);
                    break;
            }
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void NewPlayerEventSend(string username)
    {
        object[] package = new object[3];

        package[0] = username;
        package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
        package[2] = 0;

        PhotonNetwork.RaiseEvent
            (
            (byte)EventCodes.NewPlayerEvent,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
            new SendOptions { Reliability = true }
            );
    }

    public void NewPlayerEventReceive(object[] receivedData)
    {
        PlayerInformation playerInfo = new PlayerInformation((string)receivedData[0], (int)receivedData[1], (int)receivedData[2]);

        allPlayersList.Add(playerInfo);

        ListPlayerEventSend();
    }

    public void ListPlayerEventSend()
    {
        object[] package = new object[allPlayersList.Count + 1];

        package[0] = currentGameState;

        for (int i = 0; i < allPlayersList.Count; i++)
        {
            object[] playersInPackage = new object[3];

            playersInPackage[0] = allPlayersList[i].playerUsername;
            playersInPackage[1] = allPlayersList[i].playerActorNumber;
            playersInPackage[2] = allPlayersList[i].playerKills;

            package[i + 1] = playersInPackage;
        }

        PhotonNetwork.RaiseEvent
            (
            (byte)EventCodes.ListPlayersEvent,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
    }

    public void ListPlayerEventReceive(object[] receivedData)
    {
        allPlayersList.Clear();

        currentGameState = (GameStates)receivedData[0];

        for (int i = 1; i < receivedData.Length; i++)
        {
            object[] playersInPackage = (object[])receivedData[i];

            PlayerInformation player = new PlayerInformation
                (
                (string)playersInPackage[0],
                (int)playersInPackage[1],
                (int)playersInPackage[2]
                );

            allPlayersList.Add(player);

            if (PhotonNetwork.LocalPlayer.ActorNumber == player.playerActorNumber)
            {
                index = i-1;
            }
        }

        CurrentGameStateCheck();
    }

    public void UpdateStatsEventSend(int playerActorNumber, int statTypeToUpdate, int amountToChange, bool isBossDefeated)
    {
        object[] package = new object[] { playerActorNumber, statTypeToUpdate, amountToChange, isBossDefeated };

        PhotonNetwork.RaiseEvent
            (
            (byte)EventCodes.UpdateStatsEvent,
            package,
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            new SendOptions { Reliability = true }
            );
    }

    public void UpdateStatsEventReceive(object[] receivedData)
    {
        int playerActorNumber = (int)receivedData[0];
        int statTypeToUpdate = (int)receivedData[1];
        int amountToChange = (int)receivedData[2];
        bool isBossDefeated = (bool)receivedData[3];

        for (int i = 0; i < allPlayersList.Count; i++)
        {
            if(allPlayersList[i].playerActorNumber == playerActorNumber)
            {
                switch (statTypeToUpdate)
                {
                    case 0:
                        allPlayersList[i].playerKills += amountToChange;
                        Debug.Log("Player " + allPlayersList[i].playerUsername + " : kills " + allPlayersList[i].playerKills);
                        break;
                }

                if (i == index)
                {
                    UpdateStatsDisplay();
                }

                if (UIController.instance.leaderboardTableDisplay.activeInHierarchy)
                {
                    ShowLeaderboard();
                }

                break;
            }
        }

        if(isBossDefeated == true)
        {
            MatchEndCheck(isBossDefeated);
        }
    }

    public void UpdateStatsDisplay()
    {
        if (allPlayersList.Count > index)
        {
            UIController.instance.killStatText.text = "Kills: " + allPlayersList[index].playerKills;
        }
        else
        {
            UIController.instance.killStatText.text = "Kills: 0 ";
        }
    }

    void ShowLeaderboard()
    {
        UIController.instance.leaderboardTableDisplay.SetActive(true);


        foreach (LeaderboardPlayerInformation leaderboardPlayer in leaderboardPlayerInformationList)
        {
            Destroy(leaderboardPlayer.gameObject);
        }
        leaderboardPlayerInformationList.Clear();

        UIController.instance.LeaderboardPlayerInformation.gameObject.SetActive(false);


        List<PlayerInformation> SortedPlayersList = SortLeaderboardPlayers(allPlayersList);


        foreach (PlayerInformation playerToAdd in SortedPlayersList)
        {
            LeaderboardPlayerInformation newPlayerLeaderboardRow = Instantiate(UIController.instance.LeaderboardPlayerInformation, UIController.instance.LeaderboardPlayerInformation.transform.parent);

            newPlayerLeaderboardRow.SetPlayerLeaderboardInformation(playerToAdd.playerUsername, playerToAdd.playerKills);

            newPlayerLeaderboardRow.gameObject.SetActive(true);

            leaderboardPlayerInformationList.Add(newPlayerLeaderboardRow);
        }
    }

    private List<PlayerInformation> SortLeaderboardPlayers(List<PlayerInformation> _allPlayersList)
    {
        List<PlayerInformation> sortedPlayersList = new List<PlayerInformation>();

        while (sortedPlayersList.Count < _allPlayersList.Count)
        {
            int highestKillScore = -1;

            PlayerInformation selectedPlayer = _allPlayersList[0];

            foreach (PlayerInformation player in _allPlayersList)
            {
                if (!sortedPlayersList.Contains(player))
                {
                    if (player.playerKills > highestKillScore)
                    {
                        selectedPlayer = player;
                        highestKillScore = player.playerKills;
                    }
                }
            }

            sortedPlayersList.Add(selectedPlayer);
        }
        return sortedPlayersList;
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void MatchEndCheck(bool _bossDefeated)
    {
        bool bossDefeated = false;
        bossDefeated = _bossDefeated;

        if(bossDefeated == true)
        {
            if(PhotonNetwork.IsMasterClient && currentGameState != GameStates.GameEndingState)
            {
                currentGameState = GameStates.GameEndingState;
                ListPlayerEventSend();
            }
        }
    }

    void CurrentGameStateCheck()
    {
        if(currentGameState == GameStates.GameEndingState)
        {
            EndMatch();
        }
    }

    void EndMatch()
    {
        currentGameState = GameStates.GameEndingState;

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
        }

        UIController.instance.matchEndScreen.SetActive(true);
        ShowLeaderboard();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(EndMatchCoroutine());
    }

    private IEnumerator EndMatchCoroutine()
    {
        yield return new WaitForSeconds(waitGameStateTime);

        PhotonNetwork.AutomaticallySyncScene = false;

        PhotonNetwork.LeaveRoom();
    }


    [System.Serializable]
    public class PlayerInformation
    {
        public string playerUsername;
        public int playerActorNumber;
        public int playerKills;
        public PlayerInformation(string _playerUsername, int _playerActorNumber, int _playerKills)
        {
            playerUsername = _playerUsername;
            playerActorNumber = _playerActorNumber;
            playerKills = _playerKills;
        }
    }
}
