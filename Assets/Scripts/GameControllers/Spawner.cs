using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private SpawnerScriptableObject _spawnerSO;
    [SerializeField] private GameObject[] _spawnPoints;
    public bool StopWave { get; set; }

    private NetworkPrefabRef[] _enemies;
    private float _spawnDelay;

    private void Awake()
    {
        _enemies = _spawnerSO.spawnObjects;
        _spawnDelay = _spawnerSO.spawnDelay;
    }

    public enum ObjectType
    {
        Object1,
        Object2,
        Object3
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

    private void SpawnEnemy(ObjectType enemyType)
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
            SpawnEnemy(ObjectType.Object1);
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    IEnumerator SpawnWave2()
    {
        while (!StopWave)
        {
            yield return new WaitForSeconds(_spawnDelay);
            int[] enemyVariants = new int[] { (int)ObjectType.Object1, (int)ObjectType.Object3 };
            SpawnEnemy((ObjectType)Enum.ToObject(typeof(ObjectType), UnityEngine.Random.Range(enemyVariants[0], enemyVariants[enemyVariants.Length - 1] + 1)));
        }
    }

    IEnumerator SpawnWave3()
    {
        while (!StopWave)
        {
            yield return new WaitForSeconds(_spawnDelay);
            int[] enemyVariants = new int[] { (int)ObjectType.Object1, (int)ObjectType.Object2, (int)ObjectType.Object3 };
            SpawnEnemy((ObjectType)Enum.ToObject(typeof(ObjectType), UnityEngine.Random.Range(enemyVariants[0], enemyVariants[enemyVariants.Length - 1] + 1)));
        }
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }
}
