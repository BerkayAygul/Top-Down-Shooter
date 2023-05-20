using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviourPunCallbacks
{
    private EnemyShooting _enemyShooting;
    public float projectileSpeed;
    private Vector3 projectileDirection;

    public int damageToGive = 50;

    void Start()
    {
        _enemyShooting = new EnemyShooting();
        projectileDirection = _enemyShooting.GetNearestPlayer(gameObject).position - transform.position;
        projectileDirection.Normalize();
    }

    void Update()
    {
        transform.position += projectileDirection * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        if(collisionObject.tag == "Player")
        {
            PlayerAttributes player = collisionObject.gameObject.GetComponent<PlayerAttributes>();
            if (player.photonView.IsMine)
            {
                player.photonView.RPC("TakeDamage", RpcTarget.All, damageToGive);
            }
            Destroy(gameObject);
        }
        else if(collisionObject.tag == "ninjaweapon")
        {
            
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
