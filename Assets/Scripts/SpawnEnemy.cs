using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnEnemy : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups;
        public int waveQuota;
        public float spawnInterval;
        public int spawnCount;
    }
    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount;
        public int spawnCount;
        public GameObject enemyPrefab;

    }
   public List<Wave> waves;
   public int currentWaveCount;
   
   [Header("Spawner Attributes")] 
   private float spawnTimer;
   public float waveInterval;
   public int enemiesAlive;
   public int maxEnemiesAllowed;
   public bool maxEnemiesReached = false;
   
    void Start()
    {
        CalculateWaveQuota();
    }
    
    void Update()
    {
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0)
        {
            StartCoroutine(BeginNextWave());
        }
        
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0;
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                SpawnEnemies();
            }
            
        }
    }

    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(waveInterval);
        if(currentWaveCount < waves.Count-1)
        {
            currentWaveCount++;
            CalculateWaveQuota();
        }
       
    }

    void CalculateWaveQuota()
    {
        int currenwWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currenwWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currenwWaveQuota;
    }

    void SpawnEnemies()
    {
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                if (enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    //Aynı anda spawn olabilecek düşman sayısını limitler.
                    if(enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                    Vector2 spawnPosition = Random.insideUnitCircle * Vector2.one * 19.5f;
                    PhotonNetwork.InstantiateRoomObject(enemyGroup.enemyPrefab.name,spawnPosition,quaternion.identity);
                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;
                }
            }
        }

        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
    }
    
}
