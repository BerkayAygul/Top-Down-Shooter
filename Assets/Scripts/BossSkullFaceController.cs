using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Random = UnityEngine.Random;
using Pathfinding;
using System.Linq;
using System;
using Unity.Mathematics;

public class BossSkullFaceController : MonoBehaviour
{
    public Rigidbody2D enemyRB;

    private Vector3 moveDirection;

    public Animator enemyAnimator;

    public int enemyMaxHealth = 3000;
    public int currentEnemyHealth;

    public GameObject hitEffect;

    public SpriteRenderer enemySpriteRenderer;

    public PhotonView pw;

    //public bool isBoss = true;

    public AIPath aiPath;

    //public bool isHitByStunned = false;
    //public Sprite MonkhitSprite;

    public GameObject projectileObject1;
    public GameObject projectileObject2;
    public Transform projectileFirePoint1;
    public Transform projectileFirePoint2;
    public Transform projectileFirePoint3;
    public float timeBetweenShots = 1f;
    private float shotCounter = 1f;

    public float shotRange = 4f;
    public bool hasShotRange;

    public bool enemyShouldShoot;
    public bool enemyShouldMoveTowardsPlayer;
    public bool enemyShouldNecromance;

    public bool enemyShouldMoveAnimation;
    public bool enemyShouldIdleAnimation;

    public GameObject enemyToNecromance;
    public bool isNecromancing;
    private float necromanceInterval = 5f;
    private float necromanceTimer;
    private int maxNecromanceEnemyCount = 8;
    public int currentNecromanceEnemyCount;
    public GameObject necromancerVFX;
    public GameObject InstantiatedNecromancerEffect;
    public Transform necromancerVFXPosition;

    public enum BossSkullFacePhaseCodes : byte
    {
        FirstPhase,
        SecondPhase,
        ThirdPhase
    }

    public BossSkullFacePhaseCodes CurrentBossPhaseCode;

    public Dictionary<BossSkullFacePhaseCodes, Action> phaseBehaviors;

    private void Awake()
    {
        pw = GetComponent<PhotonView>();
    }

    void Start()
    {
        currentEnemyHealth = enemyMaxHealth;

        if (GetNearestPlayer(gameObject) != null)
        {
             aiPath.target = GetNearestPlayer(gameObject);
        }

        CurrentBossPhaseCode = BossSkullFacePhaseCodes.FirstPhase;

        //InitializePhaseBehaviors();
        //UpdatePhaseBehaviors(); 
        pw.RPC("InitializePhaseBehaviors", RpcTarget.All);
        pw.RPC("UpdatePhaseBehaviors", RpcTarget.All);

    }

    void Update()
    {
        phaseBehaviors[CurrentBossPhaseCode]?.Invoke();
    }

    public Transform GetNearestPlayer(GameObject gameObject)
    {
        var players = UnityEngine.Object.FindObjectsOfType<PlayerAttributes>();
        Transform nearestPlayer;
        List<float> distances = new List<float>();

        for (int i = 0; i < players.Length; i++)
        {
            distances.Add(Vector3.Distance(gameObject.transform.position, players[i].transform.position));
        }
        int index = distances.FindIndex(distance => distances.Min() == distance);// used Linq for getting the min distance value from distances list.
        nearestPlayer = players[index].transform;
        return nearestPlayer;
    }

    public float GetDistance(Transform playerTransform, GameObject gameObject)
    {
        return Vector3.Distance(gameObject.transform.position, playerTransform.position);
    }
    public void EnemyAnimationCheck()
    {
        if (CurrentBossPhaseCode == BossSkullFacePhaseCodes.FirstPhase || CurrentBossPhaseCode == BossSkullFacePhaseCodes.ThirdPhase)
        {
            enemyAnimator.SetBool("isMoving", enemyShouldMoveAnimation);
            enemyAnimator.SetBool("isIdle", !enemyShouldMoveAnimation);

            if (enemyShouldMoveAnimation)
            {
                float xVelocity = aiPath.desiredVelocity.x;
                float yVelocity = aiPath.desiredVelocity.y;

                enemyAnimator.SetBool("aimRight", xVelocity > 0 && (yVelocity == 0 || yVelocity < 0));
                enemyAnimator.SetBool("aimUpRight", xVelocity > 0 && yVelocity > 0);
                enemyAnimator.SetBool("aimLeft", xVelocity < 0 && (yVelocity == 0 || yVelocity < 0));
                enemyAnimator.SetBool("aimUpLeft", xVelocity < 0 && yVelocity > 0);
                enemyAnimator.SetBool("aimUp", xVelocity == 0 && yVelocity > 0);
                enemyAnimator.SetBool("aimDown", xVelocity == 0 && yVelocity < 0);
            }
            else if(!enemyShouldMoveAnimation)
            {
                enemyAnimator.SetBool("aimRight", false);
                enemyAnimator.SetBool("aimUpRight", false);
                enemyAnimator.SetBool("aimLeft", false);
                enemyAnimator.SetBool("aimUpLeft", false);
                enemyAnimator.SetBool("aimUp", false);
                enemyAnimator.SetBool("aimDown", true);
            }
        }
        else if(CurrentBossPhaseCode == BossSkullFacePhaseCodes.SecondPhase)
        {
            enemyAnimator.SetBool("isMoving", false);
            enemyAnimator.SetBool("isIdle", true);
            enemyAnimator.SetBool("aimRight", false);
            enemyAnimator.SetBool("aimUpRight", false);
            enemyAnimator.SetBool("aimLeft", false);
            enemyAnimator.SetBool("aimUpLeft", false);
            enemyAnimator.SetBool("aimUp", true);
            enemyAnimator.SetBool("aimDown", false);
        }
    }

    [PunRPC]
    public void BossSkullFaceTakeDamage(int damage, int damagerPlayerActorNumber)
    {
        pw.ControllerActorNr = damagerPlayerActorNumber;
        currentEnemyHealth -= damage;

            PhotonNetwork.Instantiate(hitEffect.name, transform.position, transform.rotation);
            if (currentEnemyHealth > 0)
            {
                UpdateBossPhase();
            }
            else if(currentEnemyHealth <= 0)
            {
                currentEnemyHealth = 0;

                MatchManager.instance.UpdateStatsEventSend(damagerPlayerActorNumber, 0, 50, true);

                //RandomItemAddToPlayer(damagerPlayerActorNumber);
                //GiveExp(enemyExp, damagerPlayerActorNumber);
                DestroyObject();
            }
    }

    [PunRPC]
    public void UpdatePhaseBehaviors()
    {
        switch (CurrentBossPhaseCode)
        {
            case BossSkullFacePhaseCodes.FirstPhase:
                enemyShouldMoveTowardsPlayer = true;
                enemyShouldShoot = true;
                enemyShouldNecromance = false;
                break;
            case BossSkullFacePhaseCodes.SecondPhase:
                enemyShouldMoveTowardsPlayer = false;
                enemyShouldShoot = false;
                enemyShouldNecromance = true;
                aiPath.target = null;
                aiPath.maxSpeed = 0;
                InstantiatedNecromancerEffect = PhotonNetwork.Instantiate(necromancerVFX.name, necromancerVFXPosition.position, necromancerVFXPosition.rotation);
                InstantiatedNecromancerEffect.transform.SetParent(transform);
                enemyRB.bodyType = RigidbodyType2D.Kinematic;
                break;
            case BossSkullFacePhaseCodes.ThirdPhase:
                enemyShouldMoveTowardsPlayer = true;
                enemyShouldShoot = true;
                enemyShouldNecromance = false;
                InstantiatedNecromancerEffect.SetActive(false);
                aiPath.target = GetNearestPlayer(gameObject);
                aiPath.maxSpeed = 4;
                enemyRB.bodyType = RigidbodyType2D.Dynamic;
                break;
        }
    }
    void UpdateBossPhase()
    {
        if (currentEnemyHealth <= 2000 && currentEnemyHealth > 1000 && CurrentBossPhaseCode != BossSkullFacePhaseCodes.SecondPhase)
        {
            CurrentBossPhaseCode = BossSkullFacePhaseCodes.SecondPhase;
            UpdatePhaseBehaviors();
        }
        else if (currentEnemyHealth < 1000  && currentEnemyHealth > 0 && CurrentBossPhaseCode != BossSkullFacePhaseCodes.ThirdPhase)
        {
            CurrentBossPhaseCode = BossSkullFacePhaseCodes.ThirdPhase;
            UpdatePhaseBehaviors();
        }
    }
    public void MovePathfinding()
    {
        if (enemyShouldMoveTowardsPlayer == true)
        {
                enemyAnimator.enabled = true;
                GetComponent<AIDestinationSetter>().enabled = true;
                GetComponent<AIPath>().enabled = true;

            if (aiPath.desiredVelocity.magnitude >= 0.01f)
            {
                EnemyAnimationCheck();
                enemyShouldMoveAnimation = true;
                //enemyAnimator.enabled = true;
                //GetComponent<AIDestinationSetter>().enabled = true;
                //GetComponent<AIPath>().enabled = true;
                //aiPath.target = GetNearestPlayer(gameObject);
            }
            else if (aiPath.desiredVelocity.magnitude == 0f)
            {
                EnemyAnimationCheck();
                enemyShouldMoveAnimation = false;
                //GetComponent<AIDestinationSetter>().enabled = false;
                //GetComponent<AIPath>().enabled = false;
                //aiPath.target = null;
                //enemyAnimator.enabled = false;
            }
        }


        /*if (GetNearestPlayer(gameObject) != null)
        {
            aiPath.target = GetNearestPlayer(gameObject);
        }*/
    }

    public void Shoot()
    {
        if(enemyShouldShoot)
        {
            if(CurrentBossPhaseCode == BossSkullFacePhaseCodes.FirstPhase)
            {
                if (shotCounter > 0)
                {
                    shotCounter -= Time.deltaTime;
                }
                else
                {
                    shotCounter = timeBetweenShots;
                    Instantiate(projectileObject1, projectileFirePoint1.position, projectileFirePoint1.rotation);
                }
            }
            else if(CurrentBossPhaseCode == BossSkullFacePhaseCodes.ThirdPhase)
            {
                timeBetweenShots = 0.75f;
                if (shotCounter > 0)
                {
                    shotCounter -= Time.deltaTime;
                }
                else
                {
                    shotCounter = timeBetweenShots;
                    Instantiate(projectileObject1, projectileFirePoint1.position, projectileFirePoint1.rotation);
                    Instantiate(projectileObject2, projectileFirePoint2.position, projectileFirePoint1.rotation);
                    Instantiate(projectileObject2, projectileFirePoint3.position, projectileFirePoint1.rotation);
                    //Instantiate(projectileObject, projectileFirePoint1.position, projectileFirePoint2.rotation);
                }
            }
        }
    }

    public void NecromanceEnemies()
    {
        EnemyAnimationCheck();

        if(CurrentBossPhaseCode == BossSkullFacePhaseCodes.SecondPhase && currentEnemyHealth > 0)
        {
            if (!isNecromancing && currentNecromanceEnemyCount < maxNecromanceEnemyCount)
            {
                enemyShouldNecromance = true;
                StartCoroutine(SpawnEnemiesWithInterval());
            }
        }
        else
        {
            enemyShouldNecromance = false;
        }

        //InvokeRepeating(nameof(SpawnEnemy), 2f, 2f);
    }

    private IEnumerator SpawnEnemiesWithInterval()
    {
        isNecromancing = true;
        float spawnInterval = necromanceInterval;

        while (enemyShouldNecromance && currentNecromanceEnemyCount < maxNecromanceEnemyCount)
        {
            Vector2 spawnPosition = Random.insideUnitCircle * Vector2.one * 19.5f;
            PhotonNetwork.InstantiateRoomObject(enemyToNecromance.name, spawnPosition, Quaternion.identity);
            currentNecromanceEnemyCount++;

            if (currentNecromanceEnemyCount >= maxNecromanceEnemyCount)
            {
                //isNecromancing = false;
                break;
            }

            yield return new WaitForSeconds(spawnInterval);

            spawnInterval -= 1f * Time.deltaTime;
        }

        isNecromancing = false;
    }

    [PunRPC]
    private void InitializePhaseBehaviors()
    {
        phaseBehaviors = new Dictionary<BossSkullFacePhaseCodes, Action>
        {
            { BossSkullFacePhaseCodes.FirstPhase, () => { MovePathfinding(); Shoot(); } },
            { BossSkullFacePhaseCodes.SecondPhase, NecromanceEnemies},
            { BossSkullFacePhaseCodes.ThirdPhase, () => { MovePathfinding(); Shoot(); } }
        };
    }

    public void DestroyObject()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    /*public void GiveExp(int amount, int actorNumber)
{
    foreach (var player in MatchManager.instance.playersGameObjects)
    {
        int playerActorNumber = player.GetComponent<PhotonView>().ControllerActorNr;
        PlayerAttributes playerAttributes = player.GetComponent<PlayerAttributes>();
        if (playerActorNumber == actorNumber)
        {
            playerAttributes.GetExp(amount);
        }
    }
}*/

    /*IEnumerator EnemyStun()
    {
        isHitByStunned = true;
        aiPath.target = null;
        GetComponent<AIDestinationSetter>().enabled = false;
        GetComponent<AIPath>().enabled = false;
        enemyAnimator.enabled = false;
        //enemyShouldMoveTowardsPlayer = true;
        EnemyWeapon.instance.enemyShouldShoot = false;
        yield return new WaitForSeconds(1f);
        isHitByStunned = false;
        EnemyAnimationCheck();
        enemyAnimator.enabled = true;
        aiPath.target = _enemyShooting.GetNearestPlayer(gameObject);
        GetComponent<AIDestinationSetter>().enabled = true;
        GetComponent<AIPath>().enabled = true;
        EnemyWeapon.instance.enemyShouldShoot = true;
    }*/

}
