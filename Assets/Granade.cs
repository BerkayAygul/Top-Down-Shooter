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

public class Granade : MonoBehaviour
{
    public float explodeForce;
    public float explodeRadius;
    public int damage;
    private PlayerAttributes player;

    private void OnDestroy()
    {
        GetPlayer();
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
                Debug.LogError("Damage: " + damage);
                EnemyController enemy = col.GetComponent<EnemyController>();
                col.gameObject.GetComponent<Animator>().enabled = false;
                col.gameObject.GetComponent<AIDestinationSetter>().enabled = false;
                col.gameObject.GetComponent<AIPath>().enabled = false;
                Rigidbody2D rb2d = col.GetComponent<Rigidbody2D>();
                Vector2 direction = col.transform.position - transform.position;
                enemy.pw.RPC("TakeDamage", RpcTarget.All, damage, PhotonNetwork.LocalPlayer.ActorNumber);
                rb2d.AddForce(direction.normalized * explodeForce,ForceMode2D.Impulse);
            }
            
        }
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
