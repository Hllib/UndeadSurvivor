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
    [SerializeField] private WaveScriptableObject _waveSettingSO;

    [Networked] public TickTimer roundTime { get; set; }
    private List<float> _roundTime;
    private float _restTime;

    private Dictionary<int, bool> waveCompletion;
    private GameState _gameState;
    private bool _gameStarted;
    private int _currentWaveId;

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
            _waveSettingSO.wave1Duration,
            _waveSettingSO.wave2Duration,
            _waveSettingSO.wave3Duration
        };

        _restTime = _waveSettingSO.restTime;
        _gameStarted = false;
    }

    public void StartGame()
    {
        ChooseNextRound();
        _collectablesSpawner.SpawnInitialWeapons();
        _gameStarted = true;
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
            case SpawnWave.Wave1:
                _enemySpawner.StartWave((int)SpawnWave.Wave1);
                _collectablesSpawner.StartWave((int)SpawnWave.Wave1);
                SetTimer(_roundTime[0]);
                _currentWaveId = (int)SpawnWave.Wave1;
                break;
            case SpawnWave.Wave2: 
                _enemySpawner.StartWave((int)SpawnWave.Wave2); 
                _collectablesSpawner.StartWave((int)SpawnWave.Wave2); 
                SetTimer(_roundTime[1]);
                _currentWaveId = (int)SpawnWave.Wave2; 
                break;
            case SpawnWave.Wave3: 
                _enemySpawner.StartWave((int)SpawnWave.Wave3); 
                _collectablesSpawner.StartWave((int)SpawnWave.Wave3); 
                SetTimer(_roundTime[2]);
                _currentWaveId = (int)SpawnWave.Wave3;
                break;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (roundTime.Expired(Runner) && _gameStarted)
        {
            switch (_gameState)
            {
                case GameState.Rest: ChooseNextRound(); break;
                case GameState.Round: StartRest(); SetWaveAsCompleted(); break;
            }
        }
    }

    private void SetWaveAsCompleted()
    {
        waveCompletion[_currentWaveId] = true;
    }

    public void ChooseNextRound()
    {
        var nextWave = waveCompletion.FirstOrDefault(wave => wave.Value == false);
        if (nextWave.Key != default(int))
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
