using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

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
                DestroyObject();
            }
        }
    }

    public void DestroyObject()
    {
        PhotonNetwork.Destroy(gameObject);
    }
}
