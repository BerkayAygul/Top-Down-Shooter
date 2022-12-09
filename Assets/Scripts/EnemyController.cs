using System.Collections;
using System.Collections.Generic;
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
        if(Vector3.Distance(transform.position, PlayerAttributes.instance.transform.position) < rangeToChasePlayer)
        {
            moveDirection = PlayerAttributes.instance.transform.position - transform.position;
        }
        else
        {
            moveDirection = Vector3.zero;
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

    public void DamageEnemy(int damage)
    {
        enemyHealth -= damage;

        Instantiate(hitEffect, transform.position, transform.rotation);

        if(enemyHealth <= 0)
        {
            Destroy(gameObject);
        }
           
    }
}
