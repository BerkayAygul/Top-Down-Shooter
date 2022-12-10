using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager instance;

    private void Awake()
    {
        instance = this;
    }
    public Transform[] spawnPoints;

    void Start()
    {
        foreach (Transform spawnLocation in spawnPoints)
        {
            spawnLocation.gameObject.SetActive(false);
        }
    }

    public Transform GetSpawnPoint()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}

