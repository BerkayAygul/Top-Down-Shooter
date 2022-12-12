using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D enemyRB;

    public float rangeToChasePlayer;
    private Vector3 moveDirection;

    public Animator skeletonAnimator;

    public int enemyHealth = 200;

    public GameObject hitEffect;

    void Update()
    {
        moveDirection = GetNearestPlayer().position - transform.position;
        
        
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

    public void DamageEnemy(int damage)
    {
        enemyHealth -= damage;

        Instantiate(hitEffect, transform.position, transform.rotation);

        if(enemyHealth <= 0)
        {
            Destroy(gameObject);
        }
           
    }
    
    Transform GetNearestPlayer()
    {
        var players = UnityEngine.Object.FindObjectsOfType<PlayerAttributes>();
        Transform nearestPlayer;
        List<float> distances = new List<float>();
        
        for (int i = 0; i < players.Length; i++ )
        {
            distances.Add(Vector3.Distance(gameObject.transform.position, players[i].transform.position));
        }
        int index = distances.FindIndex(distance => distances.Min() == distance);// used Linq for getting the min distance value from distances list.
        nearestPlayer = players[index].transform;
        return nearestPlayer;
    }
}
