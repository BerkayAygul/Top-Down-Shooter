using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float projectileSpeed = 7.5f;
    public Rigidbody2D projectileRB;

    public GameObject projectileImpactEffect;

    public int damageToGive = 50;

    void Update()
    {
        projectileRB.velocity = transform.right * projectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        Instantiate(projectileImpactEffect, transform.position, transform.rotation);
        Destroy(gameObject);

        if(collisionObject.tag == "Enemy")
        {
            collisionObject.GetComponent<EnemyController>().DamageEnemy(damageToGive);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
