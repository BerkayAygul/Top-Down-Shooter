using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviourPunCallbacks
{
    private EnemyShooting _enemyShooting;
    public float moveSpeed;
    public Rigidbody2D enemyRB;

    public float rangeToChasePlayer;
    private Vector3 moveDirection;

    public Animator skeletonAnimator;

    public int enemyMaxHealth = 200;
    public int currentEnemyHealth;

    public GameObject hitEffect;
    public List<ItemScriptable> possibleItems;

    public PhotonView pw;

    public bool isBoss = false;

    private void Awake()
    {
        pw = GetComponent<PhotonView>();
    }

    private void Start()
    {
        _enemyShooting = new EnemyShooting();
        currentEnemyHealth = enemyMaxHealth;
    }

    void Update()
    {
        if (_enemyShooting.GetDistance(_enemyShooting.GetNearestPlayer(gameObject).transform,gameObject) >= 4)
        {
            moveDirection = _enemyShooting.GetNearestPlayer(gameObject).position - transform.position;
        }
        else
        {
            moveDirection = Vector2.zero;
        }
        
        
        moveDirection.Normalize();

        enemyRB.velocity = moveDirection * moveSpeed;

        if (moveDirection.x > 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (moveDirection != Vector3.zero)
        {
            skeletonAnimator.SetBool("isSkeletonMoving", true);
        }
        else
        {
            skeletonAnimator.SetBool("isSkeletonMoving", false);
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, int damagerPlayerActorNumber)
    {
        pw.ControllerActorNr = damagerPlayerActorNumber;
        if (pw.IsMine)
        {
            currentEnemyHealth -= damage;

            PhotonNetwork.Instantiate(hitEffect.name, transform.position, transform.rotation);
            if (currentEnemyHealth <= 0)
            {
                currentEnemyHealth = 0;

            }
        }
        if (selectedItems.Count > 0)
        {
            ItemScriptable droppedItem = selectedItems[Random.Range(0, selectedItems.Count-1)]; //select random item
            
            Inventory playerInventory = MatchManager.instance.inventories[playerActorNumber];//getting player's  inventory
            
            GameObject itemaddText = Instantiate(MatchManager.instance.itemTextPrefab, MatchManager.instance.itemTextPanel.transform);
            itemaddText.transform.SetParent(MatchManager.instance.itemTextPanel.transform);
            itemaddText.GetComponent<TextMeshProUGUI>().text = GetPlayerName(playerActorNumber) + " isimli oyuncu " +
                                                               droppedItem.itemName + " eşyasını düşürdü"; 
            playerInventory.items.Add(ItemAttributeSetter(droppedItem));
        }
      
    }

    public string GetPlayerName(int playerActorNumber)
    {
        int i = 0;
        foreach (var playerInformation in MatchManager.instance.allPlayersList)
        {
            i++;
            
            if (playerInformation.playerActorNumber == playerActorNumber)
            {
                return playerInformation.playerUsername;
            }
        }

        return "Bir hata ortaya cikti";
    }
    public void DestroyObject()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
