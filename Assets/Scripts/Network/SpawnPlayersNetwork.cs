using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class SpawnPlayersNetwork : NetworkBehaviour
{
    [SerializeField] private NetworkPlayer _playerPrefab;

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
        Runner.Spawn(_playerPrefab, GetSpawnPos(), Quaternion.identity, Object.InputAuthority);//for fixed playerRef
    }

}
