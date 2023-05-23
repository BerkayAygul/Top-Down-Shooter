using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public static EnemyWeapon instance;

    private EnemyShooting _enemyShooting;
    public bool enemyShouldShoot;
    public GameObject projectileObject;
    public Transform projectileFirePoint;
    public float timeBetweenShots = 1f;
    private float shotCounter = 1f;

    public float shotRange = 4f;
    public bool hasShotRange;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _enemyShooting = new EnemyShooting();
    }

    void Update()
    {
        if(enemyShouldShoot == true && hasShotRange == false)
        {
            Shoot();
        }
        else if (hasShotRange && _enemyShooting.GetDistance(_enemyShooting.GetNearestPlayer(gameObject), gameObject) < shotRange)
        {
            Shoot();
        }
    }

    public void Shoot()
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
