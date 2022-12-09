using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float projectileSpeed;
    private Vector3 projectileDirection;

    void Start()
    {
        projectileDirection = PlayerAttributes.instance.transform.position - transform.position;
        projectileDirection.Normalize();
    }

    void Update()
    {
        transform.position += projectileDirection * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collisionObject)
    {
        if(collisionObject.tag == "Player")
        {

        }
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
