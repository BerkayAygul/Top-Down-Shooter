using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomBrowse : MonoBehaviour
{
    public TMP_Text roomButtonText;

    private RoomInfo roomInfo;

    public TMP_Text roomPlayerCountText;

    public void SetButtonDetails(RoomInfo inputInfo)
    {
        roomInfo = inputInfo;

        roomButtonText.text = roomInfo.Name;
        roomPlayerCountText.text = "Player Count: " + roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
    }

    public void OpenRoom()
    {
        ServerLauncher.instance.JoinRoom(roomInfo);
    }
}
