using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class SpawnPlayersNetwork : NetworkBehaviour
{
    [SerializeField] private NetworkPlayer _playerPrefab;
    [SerializeField] private NetworkPrefabRef[] _playerPrefabs;

    Vector3 GetSpawnPos()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        if (spawnPoints.Length == 0)
        {
            return Vector3.zero;
        }
        else
        {
            return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].transform.position;
        }
    }

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            var skinId = PlayerPrefs.GetInt("Skin");
            NetworkObject networkPlayerObject;
            switch (skinId)
            {
                case 1: networkPlayerObject = Runner.Spawn(_playerPrefabs[0], GetSpawnPos(), Quaternion.identity, Object.InputAuthority); break;
                case 2: networkPlayerObject = Runner.Spawn(_playerPrefabs[1], GetSpawnPos(), Quaternion.identity, Object.InputAuthority); break;
                case 3: networkPlayerObject = Runner.Spawn(_playerPrefabs[2], GetSpawnPos(), Quaternion.identity, Object.InputAuthority); break;
                default: networkPlayerObject = null; Debug.LogError("Player prefab not found"); break;
            }
        }
        //Runner.Spawn(_playerPrefab, GetSpawnPos(), Quaternion.identity, Object.InputAuthority);//for fixed playerRef
    }

}
