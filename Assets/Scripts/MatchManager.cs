using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;

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
    }
}
