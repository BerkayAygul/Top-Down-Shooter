using System;
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

    private void Start()
    {
        photonView.RPC("MoveProjectile", RpcTarget.All);
    }

    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        if(photonView.IsMine)
        {
            PhotonNetwork.Instantiate(projectileImpactEffect.name, transform.position, transform.rotation);
            Destroy(gameObject);

            photonView.RPC("DestroyObject", RpcTarget.All);

            if (collisionObject.tag == "Enemy")
            {
                EnemyController enemy = collisionObject.gameObject.GetComponent<EnemyController>(); ;
                enemy.pw.RPC("TakeDamage", RpcTarget.All, damageToGive, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    private void OnBecameInvisible()
    {
        photonView.RPC("DestroyObject", RpcTarget.All);
    }

    [PunRPC]
    public void MoveProjectile()
    {
        projectileRB.AddForce(transform.right * projectileSpeed*3,ForceMode2D.Impulse);
    }

    [PunRPC]
    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
