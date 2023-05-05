using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class CollectablesSpawner : NetworkBehaviour
{
    [SerializeField] private Weapon[] _initialWeapons;
    [SerializeField] private GameObject[] _weaponSpawnPoints;
    [SerializeField] private GameObject[] _spawnPoints;
    [SerializeField] private NetworkPrefabRef[] _collectables;
    public bool StopWave { get; set; }
    private float _spawnDelay = 3f;

    public void SpawnInitialWeapons()
    {
        int index = 0;

        for (int i = 0; i < _initialWeapons.Length; i++)
        {
            Runner.Spawn(_initialWeapons[i], _weaponSpawnPoints[index].transform.position, Quaternion.identity);
            index++;
        }
    }

    public enum CollectableType
    {
        Ammo,
        HealthKit,
        Bomb
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

    private void SpawnCollectable(CollectableType collectableType)
    {
        var spawnPoint = UnityEngine.Random.Range(0, _spawnPoints.Length);
        NetworkObject collectable = Runner.Spawn(_collectables[(int)collectableType], GetSpawnPos(), Quaternion.identity);
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
            SpawnCollectable(CollectableType.Ammo);
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    IEnumerator SpawnWave2()
    {
        while (!StopWave)
        {
            yield return new WaitForSeconds(_spawnDelay);
            int[] collectableVariants = new int[] { (int)CollectableType.Ammo, (int)CollectableType.HealthKit };
            SpawnCollectable((CollectableType)Enum.ToObject(typeof(CollectableType), UnityEngine.Random.Range(collectableVariants[0], collectableVariants[collectableVariants.Length - 1] + 1)));
        }
    }

    IEnumerator SpawnWave3()
    {
        while (!StopWave)
        {
            yield return new WaitForSeconds(_spawnDelay);
            int[] collectableVariants = new int[] { (int)CollectableType.Ammo, (int)CollectableType.HealthKit, (int)CollectableType.Bomb };
            SpawnCollectable((CollectableType)Enum.ToObject(typeof(CollectableType), UnityEngine.Random.Range(collectableVariants[0], collectableVariants[collectableVariants.Length - 1] + 1)));
        }
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
    }
}
