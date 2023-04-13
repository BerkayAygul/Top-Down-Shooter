using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject playerPrefab;

    private GameObject player;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer()
    {
        Transform spawnPoint = PlayerSpawnManager.instance.GetSpawnPoint();
        #region comment
        // Instantiate Player Prefab over the network.
        #endregion
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, spawnPoint.rotation);
        //Gets player's inventory information to MatchManager.cs
        Inventory playerInventory = player.GetComponent<Inventory>();
        int playerActorNumber = player.GetPhotonView().Controller.ActorNumber;
        MatchManager.instance.inventories.Add(playerActorNumber,playerInventory);
        MatchManager.instance.playersGameObjects.Add(player);
    }

    public void Die()
    {
        PhotonNetwork.Destroy(player);
    }
}
