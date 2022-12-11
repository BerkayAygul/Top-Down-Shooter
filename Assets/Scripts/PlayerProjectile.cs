using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviourPunCallbacks
{
    public float projectileSpeed = 7.5f;
    public Rigidbody2D projectileRB;

    public GameObject projectileImpactEffect;

    public int damageToGive = 50;

    void Update()
    {
        photonView.RPC("MoveProjectile", RpcTarget.All);
    }

    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        Instantiate(projectileImpactEffect, transform.position, transform.rotation);
        Destroy(gameObject);

        photonView.RPC("DestroyObject", RpcTarget.All);

        if(collisionObject.tag == "Enemy")
        {
            //collisionObject.GetComponent<EnemyController>().DamageEnemy(damageToGive);
            collisionObject.gameObject.GetPhotonView().RPC("DamageEnemy", RpcTarget.All, damageToGive);
        }
    }

    private void OnBecameInvisible()
    {
        photonView.RPC("DestroyObject", RpcTarget.All);
    }

    [PunRPC]
    public void MoveProjectile()
    {
        projectileRB.velocity = transform.right * projectileSpeed;
    }

    [PunRPC]
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
