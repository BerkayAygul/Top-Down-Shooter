using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using DG.Tweening;
using Pathfinding;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;

public class Granade : MonoBehaviourPunCallbacks
{
    public float explodeForce;
    public float explodeRadius;
    public int damage;
    private PlayerAttributes player;

    private void Start()
    {
        GetPlayer();
        if (photonView.IsMine)
        {
            photonView.RPC("MoveGranade",RpcTarget.All);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(photonView.IsMine && (other.CompareTag("Tilemap_Collision") || other.CompareTag("Enemy")))
        {
            Destroy(gameObject);
            photonView.RPC("DestroyGranade", RpcTarget.All);
        }
    }
    [PunRPC]
    public void DestroyGranade()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        damage = player.damage/2;
        explodeRadius = 1 + player.specialLevel/2;
        explodeForce += player.specialLevel*100;
        GameObject explosionEffect = PhotonNetwork.Instantiate("ExplosionEffect", transform.position, quaternion.identity);
        explosionEffect.transform.DOScale(Vector3.one*explodeRadius/2, 0);
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(gameObject.transform.position,explodeRadius);
        foreach (var col in enemiesInRange)
        {
            if (col.gameObject.GetComponent<EnemyController>() != null)
            {
                EnemyController enemy = col.GetComponent<EnemyController>();
                col.gameObject.GetComponent<Animator>().enabled = false;
                col.gameObject.GetComponent<AIDestinationSetter>().enabled = false;
                col.gameObject.GetComponent<AIPath>().enabled = false;
                Rigidbody2D rb2d = col.GetComponent<Rigidbody2D>();
                Vector2 direction = col.transform.position - transform.position;
                enemy.pw.RPC("TakeDamage", RpcTarget.All, damage, PhotonNetwork.LocalPlayer.ActorNumber);
                rb2d.AddForce(direction * explodeForce,ForceMode2D.Impulse);
            }
            
        }
    }

    IEnumerator ExplodeGranade()
    {
        yield return new WaitForSeconds(2f);
        
        Destroy(gameObject);
    }
    [PunRPC]
    public void MoveGranade()
    {
        gameObject.GetComponent<Rigidbody2D>().AddForce(transform.right*0.5f,ForceMode2D.Impulse);
    }
    public void GetPlayer()
    {
        foreach (var playerInList in MatchManager.instance.playersGameObjects)
        {
            if (playerInList.GetComponent<PhotonView>().ControllerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                player = playerInList.GetComponent<PlayerAttributes>();
                
            }
        }
    }
}
