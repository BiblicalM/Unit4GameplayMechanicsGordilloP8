using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public List<GameObject> powerupPrefabs;
    private float spawnRange = 9.0f;
    public int enemyCount;
    public int waveNumber = 1;
    
    void Start()
    {
        SpawnEnemyWave(waveNumber);
        int i = Random.Range(0, powerupPrefabs.Count);
        Instantiate(powerupPrefabs[i], GenerateSpawnPosition(), Quaternion.Euler(0, 0, 0));
    }

    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;

        if (enemyCount == 0)
        {
            waveNumber++;
            SpawnEnemyWave(waveNumber);
            int i = Random.Range(0, powerupPrefabs.Count);
            Instantiate(powerupPrefabs[i], GenerateSpawnPosition(), Quaternion.Euler(0, 0, 0));
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float SpawnPosX = Random.Range(-spawnRange, spawnRange);
        float SpawnPosZ = Random.Range(-spawnRange, spawnRange);
        Vector3 randomPos = new Vector3(SpawnPosX, 0.15f, SpawnPosZ);
        return randomPos;
    }
}
