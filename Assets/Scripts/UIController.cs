using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public TMP_Text killStatText;

    public GameObject leaderboardTableDisplay;
    public LeaderboardPlayerInformation LeaderboardPlayerInformation;

    public Slider healthSlider;

    public GameObject matchEndScreen;

    public TMP_Text matchTimerText;

    private void Awake()
    {
        instance = this;
    }
}
