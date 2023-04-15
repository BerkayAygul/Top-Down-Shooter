using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    public Sprite[] classSprites;

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
        PlayerAttributes playerAttributes = player.GetComponent<PlayerAttributes>();
        

        //Gets player's inventory information to MatchManager.cs
        Inventory playerInventory = player.GetComponent<Inventory>();
        int playerActorNumber = player.GetPhotonView().Controller.ActorNumber;
        MatchManager.instance.inventories.Add(playerActorNumber,playerInventory);
        MatchManager.instance.playersGameObjects.Add(player);
        
        //if there is any saved data gets saved player data if not create new save for current data.
        GetPlayerData();
        ChangeClassSprite(playerAttributes.playerClass);
    }

    public void ChangeClassSprite(PlayerData.Classes playerClass)
    {
        Debug.Log(player.gameObject.transform.GetChild(0).transform.name);
        Debug.Log(playerClass.ToString());
        SpriteRenderer playerSprite = player.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        switch (playerClass)
        {
            case PlayerData.Classes.warrior:
                playerSprite.sprite = classSprites[0];
                break;
            case PlayerData.Classes.mage:
                playerSprite.sprite = classSprites[1];
                break;
            case PlayerData.Classes.archer:
                playerSprite.sprite = classSprites[2];
                break;
        }
    }
    public void GetPlayerData()
    {
        GameObject localPlayer;
        PlayerAttributes playerAttributes = new PlayerAttributes();
        foreach (var player in MatchManager.instance.playersGameObjects)
        {
            
            if (PhotonNetwork.LocalPlayer.ActorNumber == player.GetComponent<PhotonView>().ControllerActorNr)
            {
                localPlayer = player;
                playerAttributes = player.GetComponent<PlayerAttributes>();
            }
        }

        if (playerAttributes.LoadPlayer().IsUnityNull())
        {
            playerAttributes.playerClass = ClassScriptable.instance.currentClass;
            playerAttributes.SavePlayer();
            Debug.Log("Saved");
        }
        else
        {
            playerAttributes.LoadPlayer();
        }
        
    }
    public void Die()
    {
        PhotonNetwork.Destroy(player);
    }
}
