using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerWeapon : MonoBehaviourPunCallbacks
{
    public GameObject projectileObject;
    public List<Transform> projectileFirePoints;
    public float timeBetweenShots = 0.3f;
    private float shotCounter = 1;
    private GameObject player;
    private void Start()
    {
        player = SkillTreeController.instance.GetPlayerObject();
    }

    void Update()
    {
        
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
                        InstantiatePlayerBullet();
                        shotCounter = timeBetweenShots;
                    }
                    
                }
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

    
    public void InstantiatePlayerBullet()
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
}
