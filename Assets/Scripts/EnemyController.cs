using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private EnemyShooting _enemyShooting;
    public float moveSpeed;
    public Rigidbody2D enemyRB;

    public float rangeToChasePlayer;
    private Vector3 moveDirection;

    public Animator skeletonAnimator;

    public int enemyHealth = 200;

    public GameObject hitEffect;

    private PhotonView pw;

    private void Awake()
    {
        pw = GetComponent<PhotonView>();
    }

    private void Start()
    {
        _enemyShooting = new EnemyShooting();
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

    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;
        Instantiate(hitEffect, transform.position, transform.rotation);

        if(enemyHealth <= 0)
        {
            enemyHealth = 0;

            pw.RPC("DestroyObject", RpcTarget.All);
        }
    }

    [PunRPC]
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
