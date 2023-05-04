using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private CollectablesSpawner _collectablesSpawner;

    [Networked] public TickTimer roundTime { get; set; }
    private List<float> _roundTime;
    private float _restTime = 30f;

    private Dictionary<int, bool> waveCompletion;
    private GameState _gameState;

    enum GameState
    {
        Rest,
        Round
    }

    enum SpawnWave
    {
        Wave1 = 1,
        Wave2 = 2,
        Wave3 = 3
    }

    private void Awake()
    {
        waveCompletion = new Dictionary<int, bool>()
        {
            {(int)SpawnWave.Wave1, false},
            {(int)SpawnWave.Wave2, false},
            {(int)SpawnWave.Wave3, false}
        };

        _roundTime = new List<float>()
        {
            60f,
            180f,
            300f
        };
    }

    public void StartGame()
    {
        StartRound(SpawnWave.Wave1);
        _collectablesSpawner.SpawnInitialWeapons();
    }

    public void SetTimer(float seconds)
    {
        roundTime = TickTimer.CreateFromSeconds(Runner, seconds);
    }

    private void StartRest()
    {
        _enemySpawner.StopSpawning();
        _gameState = GameState.Rest;

        SetTimer(_restTime);
    }

    private void StartRound(SpawnWave spawnWave)
    {
        _gameState = GameState.Round;

        switch (spawnWave)
        {
            case SpawnWave.Wave1: _enemySpawner.StartWave((int)SpawnWave.Wave1); SetTimer(_roundTime[0]); break;
            case SpawnWave.Wave2: _enemySpawner.StartWave((int)SpawnWave.Wave2); SetTimer(_roundTime[1]); break;
            case SpawnWave.Wave3: _enemySpawner.StartWave((int)SpawnWave.Wave3); SetTimer(_roundTime[2]); break;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (roundTime.Expired(Runner))
        {
            switch (_gameState)
            {
                case GameState.Round: ChooseNextRound(); break;
                case GameState.Rest: StartRest(); break;
            }
        }
        Debug.Log(roundTime.ToString());
    }

    public void ChooseNextRound()
    {
        var nextWave = waveCompletion.FirstOrDefault(wave => wave.Value == false);
        if(nextWave.Key != null)
        {
            var indexOfWave = nextWave.Key;
            StartRound((SpawnWave)Enum.ToObject(typeof(SpawnWave), indexOfWave));
        }
        else
        {
            Debug.Log("Finished the game");
        }
    }
}
