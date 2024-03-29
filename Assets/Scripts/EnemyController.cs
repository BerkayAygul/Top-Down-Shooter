using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using Pathfinding;

public class EnemyController : MonoBehaviourPunCallbacks
{
    private BossSkullFaceController _bossSkullFaceController;

    private EnemyShooting _enemyShooting;
    //public float moveSpeed;
    public Rigidbody2D enemyRB;

    //public float rangeToChasePlayer;
    private Vector3 moveDirection;

    public Animator enemyAnimator;

    public int enemyMaxHealth = 200;
    public int currentEnemyHealth;

    public GameObject hitEffect;
    public List<ItemScriptable> possibleItems;

    public SpriteRenderer enemySpriteRenderer;
    public Color enemyRedMonkColor = new Color(1, 0, 0);
    public Color enemyBlueMonkColor = new Color(0, 0, 1);
    public Color enemyGreenMonkColor = new Color(0, 1, 0);
    public Color defaultEnemyColor = new Color(1, 1, 1);

    //Exp amount
    public int enemyExp = 1;

    public PhotonView pw;

    public bool isBoss = false;

    public AIPath aiPath;

    public bool isMoving;
    //public Vector2 playerEnemyDistanceVector;

    public bool isRedMonk = false;
    public bool isBlueMonk = false;
    public bool isGreenMonk = false;
    public bool isOwlWarden = false;
    public bool isSkeleton = false;

    public bool isHitByStunned = false;
    public Sprite MonkhitSprite;

    public GameObject necromancingVFX;
    public GameObject InstantiatedNecromancingEffect;
    public Transform necromancingVFXPosition;

    int killerPlayerActorNumber;
    int nonKillerPlayerActorNumber;
    private void Awake()
    {
        pw = GetComponent<PhotonView>();
    }

    private void Start()
    {
        _enemyShooting = new EnemyShooting();
        currentEnemyHealth = enemyMaxHealth;

        if(_enemyShooting.GetNearestPlayer(gameObject) != null)
        {
            aiPath.target = _enemyShooting.GetNearestPlayer(gameObject);
        }

        if(isSkeleton == true)
        {
            _bossSkullFaceController = FindObjectOfType<BossSkullFaceController>();
            InstantiatedNecromancingEffect = PhotonNetwork.Instantiate(necromancingVFX.name, necromancingVFXPosition.position, necromancingVFXPosition.rotation);
        }
    }

    void Update()
    {
        #region old_movement
        /*
        if (_enemyShooting.GetDistance(_enemyShooting.GetNearestPlayer(gameObject).transform, gameObject) >= 4)
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

        */
        #endregion

        EnemyMovement();
        EnemyTypeController();

        //aiPath.whenCloseToDestination = CloseToDestinationMode.Stop;
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
                if(isBoss == false)
                {
                    MatchManager.instance.UpdateStatsEventSend(damagerPlayerActorNumber, 0, 1, false);
                }
                else
                {
                    MatchManager.instance.UpdateStatsEventSend(damagerPlayerActorNumber, 0, 1, true);
                }
                RandomItemAddToPlayer(damagerPlayerActorNumber);
                //GiveExp(enemyExp,damagerPlayerActorNumber);
                photonView.RPC("GiveExp", RpcTarget.All, enemyExp, damagerPlayerActorNumber);
                DestroyObject();
            }
        }
    }
    
    private void RandomItemAddToPlayer( int playerActorNumber)
    {
        int randomNumber = Random.Range(1, 101);
        List<ItemScriptable> selectedItems = new List<ItemScriptable>();
        foreach (ItemScriptable item in possibleItems)
        {
            if (randomNumber <= item.itemDropChance)
            {
                selectedItems.Add(item);
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
    public Item ItemAttributeSetter(ItemScriptable itemScriptable)
    {
        Item item = new Item();
        item.agility = itemScriptable.agility;
        item.strenght = itemScriptable.strenght;
        item.intelligent = itemScriptable.intelligent;
        item.vitality = itemScriptable.vitality;
        item.itemName = itemScriptable.itemName;
        item.itemRarity = itemScriptable.itemRarity;
        item.itemType = itemScriptable.itemType;
        item.itemImage = itemScriptable.itemImage;
        item.itemSprite = itemScriptable.itemSprite;
        item.itemLevel = itemScriptable.itemLevel;
        item.itemDropChance = itemScriptable.itemDropChance;
        item.requiredClass = itemScriptable.requiredClass;
        return item;
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

    [PunRPC]
    public void GiveExp(int amount, int _killerPlayerActorNumber)
    {
        int totalPlayers = MatchManager.instance.allPlayersList.Count;
        
        foreach (var player in MatchManager.instance.playersGameObjects)
        {
            int getPlayerActorNumber = player.GetComponent<PhotonView>().ControllerActorNr;
            PlayerAttributes playerAttributes = player.GetComponent<PlayerAttributes>();

            if(getPlayerActorNumber == _killerPlayerActorNumber)
            {
                if(getPlayerActorNumber == playerAttributes.photonView.ControllerActorNr)
                {
                    killerPlayerActorNumber = getPlayerActorNumber;
                    playerAttributes.GetExp(amount);
                }

            }
            else if(getPlayerActorNumber != _killerPlayerActorNumber && totalPlayers == 2)
            {
                if(getPlayerActorNumber == playerAttributes.photonView.ControllerActorNr)
                {
                    nonKillerPlayerActorNumber = getPlayerActorNumber;
                    playerAttributes.GetExp(amount/2);
                }
            }
        }
    }
    public void DestroyObject()
    {
        if(isSkeleton)
        {
            if(_bossSkullFaceController != null)
            {
                _bossSkullFaceController.currentNecromanceEnemyCount--;
            }
            else
            {
                Debug.LogWarning("BossSkullFaceController not found! But still spawned skeleton should be destroyed");
            }
        }
        PhotonNetwork.Destroy(gameObject);
    }

    public void EnemyAnimationCheck()
    {
        if (isMoving == true && !isSkeleton)
        {
            enemyAnimator.SetBool("isMoving", true);
            enemyAnimator.SetBool("isIdle", false);

            if (aiPath.desiredVelocity.x > 0 && (aiPath.desiredVelocity.y == 0 || aiPath.desiredVelocity.y < 0))
            {
                enemyAnimator.SetBool("aimRight", true);
                enemyAnimator.SetBool("aimUpRight", false);
                enemyAnimator.SetBool("aimLeft", false);
                enemyAnimator.SetBool("aimUpLeft", false);
                enemyAnimator.SetBool("aimUp", false);
                enemyAnimator.SetBool("aimDown", false);

            }
            else if (aiPath.desiredVelocity.x > 0 && aiPath.desiredVelocity.y > 0)
            {
                enemyAnimator.SetBool("aimRight", false);
                enemyAnimator.SetBool("aimUpRight", true);
                enemyAnimator.SetBool("aimLeft", false);
                enemyAnimator.SetBool("aimUpLeft", false);
                enemyAnimator.SetBool("aimUp", false);
                enemyAnimator.SetBool("aimDown", false);
            }
            /*else if (enemyPositions.x > 0 && enemyPositions.y < 0)
            {
                enemyAnimator.SetBool("aimUp", false);
                enemyAnimator.SetBool("aimDown", false);
                enemyAnimator.SetBool("aimRight", false);
                enemyAnimator.SetBool("aimLeft", false);
                enemyAnimator.SetBool("aimUpRight", false);
                enemyAnimator.SetBool("aimUpLeft", false);
            }*/
            else if (aiPath.desiredVelocity.x < 0 && (aiPath.desiredVelocity.y == 0 || aiPath.desiredVelocity.y < 0))
            {
                enemyAnimator.SetBool("aimRight", false);
                enemyAnimator.SetBool("aimUpRight", false);
                enemyAnimator.SetBool("aimLeft", true);
                enemyAnimator.SetBool("aimUpLeft", false);
                enemyAnimator.SetBool("aimUp", false);
                enemyAnimator.SetBool("aimDown", false);
            }
            else if (aiPath.desiredVelocity.x < 0 && aiPath.desiredVelocity.y > 0)
            {
                enemyAnimator.SetBool("aimRight", false);
                enemyAnimator.SetBool("aimUpRight", false);
                enemyAnimator.SetBool("aimLeft", false);
                enemyAnimator.SetBool("aimUpLeft", true);
                enemyAnimator.SetBool("aimUp", false);
                enemyAnimator.SetBool("aimDown", false);
            }
            /*else if (enemyPositions.x < 0 && enemyPositions.y < 0)
            {
                enemyAnimator.SetBool("aimUp", false);
                enemyAnimator.SetBool("aimDown", false);
                enemyAnimator.SetBool("aimRight", false);
                enemyAnimator.SetBool("aimLeft", false);
                enemyAnimator.SetBool("aimUpRight", false);
                enemyAnimator.SetBool("aimUpLeft", false);

            }*/
            else if (aiPath.desiredVelocity.x == 0 && aiPath.desiredVelocity.y > 0)
            {
                enemyAnimator.SetBool("aimRight", false);
                enemyAnimator.SetBool("aimUpRight", false);
                enemyAnimator.SetBool("aimLeft", false);
                enemyAnimator.SetBool("aimUpLeft", false);
                enemyAnimator.SetBool("aimUp", true);
                enemyAnimator.SetBool("aimDown", false);
            }
            else if (aiPath.desiredVelocity.x == 0 && aiPath.desiredVelocity.y < 0)
            {
                enemyAnimator.SetBool("aimRight", false);
                enemyAnimator.SetBool("aimUpRight", false);
                enemyAnimator.SetBool("aimLeft", false);
                enemyAnimator.SetBool("aimUpLeft", false);
                enemyAnimator.SetBool("aimUp", false);
                enemyAnimator.SetBool("aimDown", true);
            }
        }
        else if(isMoving == false && !isSkeleton)
        {
            enemyAnimator.SetBool("isMoving", false);
            enemyAnimator.SetBool("isIdle", true);
            enemyAnimator.SetBool("aimRight", false);
            enemyAnimator.SetBool("aimUpRight", false);
            enemyAnimator.SetBool("aimLeft", false);
            enemyAnimator.SetBool("aimUpLeft", false);
            enemyAnimator.SetBool("aimUp", false);
            enemyAnimator.SetBool("aimDown", true);
        }
        else if(isSkeleton)
        {
            if (aiPath.desiredVelocity != Vector3.zero)
            {
                if(aiPath.desiredVelocity.x >= 0.01f)
                {
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                }
                else if(aiPath.desiredVelocity.x <= -0.01f)
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                }
                enemyAnimator.SetBool("isSkeletonMoving", true);
            }
            else
            {
                enemyAnimator.SetBool("isSkeletonMoving", false);
            } 
        }
    }

    public void EnemyTypeController()
    {
        if(!isHitByStunned)
        {
            if (isRedMonk == true)
            {
                EnemyWeapon.instance.enemyShouldShoot = true;
                if (isMoving == false)
                {
                    EnemyWeapon.instance.shotRange = 0f;
                    EnemyWeapon.instance.timeBetweenShots = 0.75f;
                    enemySpriteRenderer = GetComponent<SpriteRenderer>();
                    enemySpriteRenderer.color = enemyRedMonkColor;

                    //EnemyWeapon.instance.enemyShouldShoot = true;
                    //EnemyWeapon.instance.shotRange = aiPath.endReachedDistance + 2f;
                }
                else if (isMoving == true)
                {
                    EnemyWeapon.instance.shotRange = 10f;
                    EnemyWeapon.instance.timeBetweenShots = 2f;
                    enemySpriteRenderer = GetComponent<SpriteRenderer>();
                    enemySpriteRenderer.color = defaultEnemyColor;
                }
            }
            else if (isBlueMonk == true)
            {
                EnemyWeapon.instance.enemyShouldShoot = true;
                if (isMoving == false)
                {
                    EnemyWeapon.instance.shotRange = 0f;
                    EnemyWeapon.instance.timeBetweenShots = 1.5f;
                    enemySpriteRenderer = GetComponent<SpriteRenderer>();
                    enemySpriteRenderer.color = enemyBlueMonkColor;

                    //EnemyWeapon.instance.enemyShouldShoot = true;
                    //EnemyWeapon.instance.shotRange = aiPath.endReachedDistance + 2f;
                }
                else if (isMoving == true)
                {
                    EnemyWeapon.instance.shotRange = 10f;
                    EnemyWeapon.instance.timeBetweenShots = 1f;
                    enemySpriteRenderer = GetComponent<SpriteRenderer>();
                    enemySpriteRenderer.color = defaultEnemyColor;
                }
            }
            else if (isGreenMonk == true)
            {
                EnemyWeapon.instance.enemyShouldShoot = true;
                if (isMoving == false)
                {
                    EnemyWeapon.instance.shotRange = 0f;
                    EnemyWeapon.instance.timeBetweenShots = 1f;
                    enemySpriteRenderer = GetComponent<SpriteRenderer>();
                    enemySpriteRenderer.color = enemyGreenMonkColor;

                    //EnemyWeapon.instance.enemyShouldShoot = true;
                    //EnemyWeapon.instance.shotRange = aiPath.endReachedDistance + 2f;
                }
                else if (isMoving == true)
                {
                    EnemyWeapon.instance.shotRange = 10f;
                    EnemyWeapon.instance.timeBetweenShots = 1f;
                    enemySpriteRenderer = GetComponent<SpriteRenderer>();
                    enemySpriteRenderer.color = defaultEnemyColor;
                }
            }
            else if(isSkeleton == true)
            {
                //aiPath.target = null;
                EnemyWeapon.instance.enemyShouldShoot = true;
                EnemyWeapon.instance.shotRange = 0f;
                EnemyWeapon.instance.timeBetweenShots = 1f;
            }
        }
        else if(isHitByStunned)
        {
            if(isBlueMonk || isRedMonk || isGreenMonk)
            {
                EnemyWeapon.instance.shotRange = 10f;
                EnemyWeapon.instance.timeBetweenShots = 10f;
                enemySpriteRenderer = GetComponent<SpriteRenderer>();
                enemySpriteRenderer.color = Color.magenta;
                enemySpriteRenderer.sprite = MonkhitSprite;
            }
        }
    }
    public void EnemyMovement()
    {
        if (!isHitByStunned)
        {
            if (enemyRB.velocity.x <= 0 && enemyRB.velocity.y <= 0)
            {
                if (enemyAnimator.enabled == false)
                {
                    enemyAnimator.enabled = true;
                    GetComponent<AIDestinationSetter>().enabled = true;
                    GetComponent<AIPath>().enabled = true;
                }
            }

            if (aiPath.desiredVelocity.magnitude >= 0.01f)
            {
                isMoving = true;
                EnemyAnimationCheck();
            }
            else if (aiPath.desiredVelocity.magnitude == 0f)
            {
                isMoving = false;
                EnemyAnimationCheck();

            }

            if (_enemyShooting.GetNearestPlayer(gameObject) != null)
            {
                aiPath.target = _enemyShooting.GetNearestPlayer(gameObject);
            }
        }
    }

    IEnumerator EnemyStun()
    {
        isHitByStunned = true;
        aiPath.target = null;
        GetComponent<AIDestinationSetter>().enabled = false;
        GetComponent<AIPath>().enabled = false;
        enemyAnimator.enabled = false;
        //isMoving = true;
        EnemyWeapon.instance.enemyShouldShoot = false;
        yield return new WaitForSeconds(1f);
        isHitByStunned = false;
        EnemyAnimationCheck();
        enemyAnimator.enabled = true;
        aiPath.target = _enemyShooting.GetNearestPlayer(gameObject);
        GetComponent<AIDestinationSetter>().enabled = true;
        GetComponent<AIPath>().enabled = true;
        EnemyWeapon.instance.enemyShouldShoot = true;
    }
}
