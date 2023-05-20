using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Photon.Pun;

public class PlayerWeapon : MonoBehaviourPunCallbacks
{
    public GameObject projectileObject;
    public List<Transform> projectileFirePoints;
    public float timeBetweenShots = 0.3f;
    private float shotCounter = 1;
    private GameObject player;
    private PlayerAttributes playerAttributes;
    private Rigidbody2D projectileRB;
    private float ninjaWeaponSpeed = 4.5f;
    private Vector2 ninjaWeaponBasePosition;
    private bool isNinjaWeaponOnPlayer = true;
    private void Start()
    {
        player = SkillTreeController.instance.GetPlayerObject();
        playerAttributes = player.GetComponent<PlayerAttributes>();
        projectileRB = GetComponent<Rigidbody2D>();
        ninjaWeaponBasePosition = transform.localPosition;
    }

    void Update()
    {
        if (isNinjaWeaponOnPlayer)
        {
            transform.localPosition = ninjaWeaponBasePosition;
        }
        if(photonView.IsMine)
        {
            if (shotCounter > 0)
            {
                shotCounter -= Time.deltaTime;
            }
            else
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                {
                    if (player.GetComponent<PlayerAttributes>().playerClass == PlayerData.Classes.gunner)
                    {
                        SetBulletPoints();
                        GunnerShooting();
                        shotCounter = timeBetweenShots;
                    }
                    else if (player.GetComponent<PlayerAttributes>().playerClass == PlayerData.Classes.ninja && isNinjaWeaponOnPlayer)
                    {
                        SetNinjaWeaponScale();
                        NinjaShooting();
                        shotCounter = timeBetweenShots;
                    }
                    
                    
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        if(photonView.IsMine)
        {
            PhotonNetwork.Instantiate(projectileObject.GetComponent<PlayerProjectile>().projectileImpactEffect.name, transform.position, transform.rotation);
            
            if (collisionObject.tag == "Enemy" && playerAttributes.playerClass == PlayerData.Classes.ninja)
            {
                EnemyController enemy = collisionObject.gameObject.GetComponent<EnemyController>();
                enemy.pw.RPC("TakeDamage", RpcTarget.All, player.GetComponent<PlayerAttributes>().damage/2, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    /*
    public void InstantiatePlayerBullet()
    {
        photonView.RPC("RPCInstantiatePlayerBullet", RpcTarget.All);
    }

    [PunRPC]
    public void RPCInstantiatePlayerBullet()
    {
        PhotonNetwork.Instantiate(projectileObject.name, projectileFirePoint.position, projectileFirePoint.rotation);
    }
    */
    public void SetBulletPoints()
    {
        Debug.Log("specialLevel = " + PlayerAttributes.instance.specialLevel);
        switch (player.GetComponent<PlayerAttributes>().specialLevel)
        {
            case 1:
                projectileFirePoints[0].gameObject.SetActive(true);
                break;
            case 2:
                projectileFirePoints[0].gameObject.SetActive(false);
                projectileFirePoints[1].gameObject.SetActive(true);
                projectileFirePoints[2].gameObject.SetActive(true);
                break;
            case 3:
                projectileFirePoints[0].gameObject.SetActive(true);
                break;
            case 4:
                projectileFirePoints[3].gameObject.SetActive(true);
                break;
            case 5:
                projectileFirePoints[4].gameObject.SetActive(true);
                break;
        }
    }
    public void SetNinjaWeaponScale()
    {
        
        switch (player.GetComponent<PlayerAttributes>().specialLevel)
        {
            case 1:
                gameObject.transform.localScale = Vector3.one * 1f;
                break;
            case 2:
                gameObject.transform.DOScale(Vector3.one * 1.5f, 0);
                break;
            case 3:
                gameObject.transform.DOScale(Vector3.one * 2f, 0);
                break;
            case 4:
                gameObject.transform.DOScale(Vector3.one * 2.5f, 0);
                break;
            case 5:
                gameObject.transform.DOScale(Vector3.one * 3f, 0);
                break;
        }
    }

    
    public void GunnerShooting()
    {
        foreach (var point in projectileFirePoints)
        {
            if (point.gameObject.activeInHierarchy)
            {
                Vector3 direction = (point.localRotation * Vector2.right).normalized;
                PhotonNetwork.Instantiate(projectileObject.name, point.position, point.rotation);
            }
        }
        
    }
    public void NinjaShooting()
    {
        isNinjaWeaponOnPlayer = false;
        StartCoroutine(MoveProjectile());
    }
    public IEnumerator MoveProjectile()
    {
        projectileRB.AddForce(transform.right * ninjaWeaponSpeed*3,ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        projectileRB.velocity = Vector3.zero;
        transform.DOLocalMove(new Vector3(ninjaWeaponBasePosition.x, ninjaWeaponBasePosition.y, 0), 0.5f).OnComplete(
            () =>
            {
                isNinjaWeaponOnPlayer = true;
                StopCoroutine(MoveProjectile());
            });
        
    }
}
