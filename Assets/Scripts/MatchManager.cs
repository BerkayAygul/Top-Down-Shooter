using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class MatchManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public static MatchManager instance;

    public List<PlayerInformation> allPlayersList = new List<PlayerInformation>();

    private int index;

    public enum EventCodes : byte
    {
        NewPlayerEvent,
        ListPlayersEvent,
        UpdateStatsEvent
    }

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
        object[] package = new object[allPlayersList.Count];

        for (int i = 0; i < allPlayersList.Count; i++)
        {
            object[] playersInPackage = new object[3];

            playersInPackage[0] = allPlayersList[i].playerUsername;
            playersInPackage[1] = allPlayersList[i].playerActorNumber;
            playersInPackage[2] = allPlayersList[i].playerKills;

            package[i] = playersInPackage;
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

        for (int i = 0; i < receivedData.Length; i++)
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
                index = i;
            }
        }
    }

    public void UpdateStatsEventSend(int playerActorNumber, int statTypeToUpdate, int amountToChange)
    {
        object[] package = new object[] { playerActorNumber, statTypeToUpdate, amountToChange };

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

                break;
            }
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
