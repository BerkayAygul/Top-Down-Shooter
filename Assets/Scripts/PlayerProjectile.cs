using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviourPunCallbacks
{
    public float projectileSpeed = 7.5f;
    public Rigidbody2D projectileRB;
    private GameObject player;
    private PlayerAttributes playerAttributes;

    public GameObject projectileImpactEffect;

    private void Start()
    {
        GetPlayer();
        if(photonView.IsMine)
        {
            playerAttributes = player.GetComponent<PlayerAttributes>();
            photonView.RPC("MoveProjectile", RpcTarget.All);
        }
    }

    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        if(photonView.IsMine /*&& /*collisionObject.CompareTag("Tilemap_Collision")*/)
        {
            PhotonNetwork.Instantiate(projectileImpactEffect.name, transform.position, transform.rotation);
            Destroy(gameObject);

            photonView.RPC("DestroyObject", RpcTarget.All);

            if (collisionObject.tag == "Enemy")
            {
                EnemyController enemy = collisionObject.gameObject.GetComponent<EnemyController>();
                Debug.Log("Damage: " + playerAttributes.damage);
                enemy.pw.RPC("TakeDamage", RpcTarget.All, playerAttributes.damage, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else if (collisionObject.tag == "EnemyBossSkullFace")
            {
                BossSkullFaceController skullfaceBoss = collisionObject.gameObject.GetComponent<BossSkullFaceController>();
                Debug.Log("Damage: " + playerAttributes.damage);
                skullfaceBoss.pw.RPC("BossSkullFaceTakeDamage", RpcTarget.All, playerAttributes.damage, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    private void OnBecameInvisible()
    {
        if(photonView.IsMine)
        {
            photonView.RPC("DestroyObject", RpcTarget.All);
        }
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
    private void GetPlayer()
    {
        foreach (var playerInList in MatchManager.instance.playersGameObjects)
        {
            if(playerInList != null)
            {
                if (playerInList.GetComponent<PhotonView>().ControllerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    player = playerInList;

                }
            }
        }
    }
}
