using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef[] _enemies;
    [SerializeField] private GameObject[] _spawnPoints;
    private float _spawnDelay = 3.5f;
    public bool StopWave { get; set; }

    public enum EnemyType
    {
        WeakZombie = 0,
        StrongZombie = 1,
        Skelet = 2
    }

    public enum SpawnWave
    {
        Wave1 = 1,
        Wave2 = 2,
        Wave3 = 3
    }

    public void StartWave(int waveId)
    {
        StartSpawning((SpawnWave)Enum.ToObject(typeof(SpawnWave), waveId));
    }

    private void SpawnEnemy(EnemyType enemyType)
    {
        var spawnPoint = UnityEngine.Random.Range(0, _spawnPoints.Length);
        NetworkObject enemy = Runner.Spawn(_enemies[(int)enemyType], GetSpawnPos(), Quaternion.identity);
    }

    Vector3 GetSpawnPos()
    {
        if (_spawnPoints.Length == 0)
        {
            return Vector3.zero;
        }
        else
        {
            return _spawnPoints[UnityEngine.Random.Range(0, _spawnPoints.Length)].transform.position;
        }
    }

    private void StartSpawning(SpawnWave wave)
    {
        switch (wave)
        {
            case SpawnWave.Wave1: StartCoroutine(SpawnWave1()); break;
            case SpawnWave.Wave2: StartCoroutine(SpawnWave2()); break;
            case SpawnWave.Wave3: StartCoroutine(SpawnWave3()); break;
        }
    }

    IEnumerator SpawnWave1()
    {
        while (!StopWave)
        {
            SpawnEnemy(EnemyType.WeakZombie);
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    IEnumerator SpawnWave2()
    {
        while (!StopWave)
        {
            yield return new WaitForSeconds(_spawnDelay);
            int[] enemyVariants = new int[] { (int)EnemyType.WeakZombie, (int)EnemyType.Skelet };
            SpawnEnemy((EnemyType)Enum.ToObject(typeof(EnemyType), UnityEngine.Random.Range(enemyVariants[0], enemyVariants[enemyVariants.Length - 1] + 1)));
        }
    }

    IEnumerator SpawnWave3()
    {
        while (!StopWave)
        {
            yield return new WaitForSeconds(_spawnDelay);
            int[] enemyVariants = new int[] { (int)EnemyType.WeakZombie, (int)EnemyType.StrongZombie, (int)EnemyType.Skelet };
            SpawnEnemy((EnemyType)Enum.ToObject(typeof(EnemyType), UnityEngine.Random.Range(enemyVariants[0], enemyVariants[enemyVariants.Length - 1] + 1)));
        }
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }
}
