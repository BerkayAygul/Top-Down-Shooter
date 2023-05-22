using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnemyDestroy : MonoBehaviour
{
    
    private void OnDestroy()
    {
        SpawnEnemy spawnEnemy = FindObjectOfType<SpawnEnemy>();
        spawnEnemy.OnEnemyKilled();
    }
}
