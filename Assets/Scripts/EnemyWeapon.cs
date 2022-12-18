using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{

    private EnemyShooting _enemyShooting;
    public bool enemyShouldShoot;
    public GameObject projectileObject;
    public Transform projectileFirePoint;
    public float timeBetweenShots = 1f;
    private float shotCounter = 1f;

    public float shootRange = 4f;

    private void Start()
    {
        _enemyShooting = new EnemyShooting();
    }

    void Update()
    {
        if(enemyShouldShoot && _enemyShooting.GetDistance(_enemyShooting.GetNearestPlayer(gameObject),gameObject) < shootRange)
        {
            if (shotCounter > 0)
            {
                shotCounter -= Time.deltaTime;
            }
            else
            {
                shotCounter = timeBetweenShots;
                Instantiate(projectileObject, projectileFirePoint.position, projectileFirePoint.rotation);
            }
        }
    }
}
