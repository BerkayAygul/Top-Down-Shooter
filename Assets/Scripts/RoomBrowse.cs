using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomBrowse : MonoBehaviour
{
    public TMP_Text roomButtonText;

    private RoomInfo roomInfo;

    public void SetButtonDetails(RoomInfo inputInfo)
    {
        roomInfo = inputInfo;

        roomButtonText.text = roomInfo.Name;
    }

    public void OpenRoom()
    {
        ServerLauncher.instance.JoinRoom(roomInfo);
    }
}
