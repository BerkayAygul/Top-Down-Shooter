using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardPlayerInformation : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text playerKillsText;
    public void SetPlayerLeaderboardInformation(string playerName, int playerKills)
    {
        playerNameText.text = playerName;
        playerKillsText.text = playerKills.ToString();
    }
}
