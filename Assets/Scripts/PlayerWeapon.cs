using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerWeapon : MonoBehaviourPunCallbacks
{
    public GameObject projectileObject;
    public Transform projectileFirePoint;
    public float timeBetweenShots = 0.3f;
    private float shotCounter = 1;
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
                    Instantiate(projectileObject, projectileFirePoint.position, projectileFirePoint.rotation);
                    shotCounter = timeBetweenShots;
                }
            }
        }
    }
}
