using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public float projectileSpeed = 7.5f;
    public Rigidbody2D projectileRB;

    void Update()
    {
        projectileRB.velocity = transform.right * projectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
