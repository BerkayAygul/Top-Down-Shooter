using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public TMP_Text killStatText;

    public GameObject leaderboardTableDisplay;
    public LeaderboardPlayerInformation LeaderboardPlayerInformation;

    public Slider healthSlider;

    public GameObject matchEndScreen;

    public TMP_Text matchTimerText;

    public GameObject skillChoosePanel;

    public GameObject inGameMenu;

    public GameObject GameOverScreen;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowAndHideInGameMenu();
        }
    }

    public void ShowAndHideInGameMenu()
    {
        if (!inGameMenu.activeInHierarchy)
        {
            inGameMenu.SetActive(true);
        }
        else
        {
            inGameMenu.SetActive(false);
        }
    }

    public void ReturnToMainMenu()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LeaveRoom();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
