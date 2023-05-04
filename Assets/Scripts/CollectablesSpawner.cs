using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CollectablesSpawner : NetworkBehaviour
{
    [SerializeField] private Weapon[] _initialWeapons;
    [SerializeField] private GameObject[] _spawnPoints;

    public void SpawnInitialWeapons()
    {
        int index = 0;

        for (int i = 0; i < _initialWeapons.Length; i++)
        {
            Runner.Spawn(_initialWeapons[i], _spawnPoints[index].transform.position, Quaternion.identity);
            index++;
        }
    }
}
