using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject[] enemyPrefabs;
    public int enemyIndex;
    private GameObject createdEnemy;

    void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
        {
            SpawnEnemy();
        }
    }
    
    public void SpawnEnemy()
    {
        for (enemyIndex = 0; enemyIndex < enemyPrefabs.Length; enemyIndex++)
        {
            Transform spawnPoint = EnemySpawnManager.instance.GetSpawnPoint();
            createdEnemy = PhotonNetwork.Instantiate(enemyPrefabs[enemyIndex].name, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
