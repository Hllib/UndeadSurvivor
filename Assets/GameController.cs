using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    [SerializeField] private EnemySpawner _enemySpawner;

    enum SpawnWave
    {
        Wave1 = 1,
        Wave2 = 2,
        Wave3 = 3
    }

    public void StartGame()
    {
        _enemySpawner.StartWave((int)SpawnWave.Wave1);
    }
}