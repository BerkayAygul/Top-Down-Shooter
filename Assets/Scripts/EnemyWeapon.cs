using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public bool enemyShouldShoot;
    public GameObject projectileObject;
    public Transform projectileFirePoint;
    public float timeBetweenShots = 1f;
    private float shotCounter = 1f;

    public float shootRange;

    void Update()
    {
        if(enemyShouldShoot && Vector3.Distance(transform.position, PlayerAttributes.instance.transform.position) < shootRange)
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
