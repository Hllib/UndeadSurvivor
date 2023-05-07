using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : NetworkBehaviour
{
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private CollectablesSpawner _collectablesSpawner;
    [SerializeField] private WaveScriptableObject _waveSettingSO;

    public event EventHandler OnGameFinished;

    private List<float> _roundTime;
    private float _restTime;

    private Dictionary<int, bool> waveCompletion;
    private GameState _gameState;
    private bool _gameStarted;
    private int _currentWaveId;

    //--------------TIMER------------//
    private float _timeLeft;
    private bool _isTimerOn = false;
    public TextMeshProUGUI timerText;

    private void Update()
    {
        if (_isTimerOn)
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= Time.deltaTime;
                RPC_UpdateTimer(_timeLeft);
            }
            else
            {
                _timeLeft = 0;
                _isTimerOn = false;
            }
        }
    }
    [Rpc]
    private void RPC_UpdateTimer(float currentTime)
    {
        if (Object.HasStateAuthority)
        {
            currentTime += 1;
            float minutes = Mathf.FloorToInt(currentTime / 60);
            float seconds = Mathf.FloorToInt(currentTime % 60);

            timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }
    }

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
        if (Object.HasStateAuthority)
        {
            ChooseNextRound();
            _collectablesSpawner.SpawnInitialWeapons();
            _gameStarted = true;
            _isTimerOn = true;
        }
    }

    private void SetTimer(float seconds)
    {
        _timeLeft = seconds;
        _isTimerOn = true;
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
        if (!_isTimerOn && _gameStarted)
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

    private void ChooseNextRound()
    {
        var nextWave = waveCompletion.FirstOrDefault(wave => wave.Value == false);
        if (nextWave.Key != default(int))
        {
            var indexOfWave = nextWave.Key;
            StartRound((SpawnWave)Enum.ToObject(typeof(SpawnWave), indexOfWave));
        }
        else
        {
            _gameStarted = false;
            OnGameFinished?.Invoke(this, EventArgs.Empty);
        }
    }
}
