using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public TMP_Text killStatText;

    public GameObject leaderboardTableDisplay;
    public LeaderboardPlayerInformation LeaderboardPlayerInformation;

    private void Awake()
    {
        instance = this;
    }
}
