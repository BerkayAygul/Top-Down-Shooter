using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager instance;

    public int spawnPointIndex = 0;

    public Transform[] spawnPoints;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach (Transform spawnLocation in spawnPoints)
        {
            spawnLocation.gameObject.SetActive(false);
        }
    }
    
    public void GoThroughSpawnPoints()
    {
        spawnPointIndex = Random.Range(0, spawnPoints.Length);
    }

    public Transform GetSpawnPoint()
    {
        GoThroughSpawnPoints();
        return spawnPoints[spawnPointIndex];
    }
}
