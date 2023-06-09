using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : NetworkBehaviour
{
    [SerializeField] private Spawner _enemySpawner;
    [SerializeField] private Spawner _collectablesSpawner;
    [SerializeField] private WaveScriptableObject _waveSettingSO;

    public event EventHandler OnGameFinished;
    public event EventHandler OnGameStarted;

    private List<float> _roundTime;
    private float _restTime;

    private Dictionary<int, bool> waveCompletion;
    private GameState _gameState;
    private bool _gameStarted;
    private int _currentWaveId;

    [Networked(OnChanged = nameof(OnTimerChanged))]
    public float TimeLeft { get; set; }

    private bool _isTimerOn { get; set; }
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI GameModeHeader;
    private readonly string gameModeHeaderRest = "Mode: rest";
    private readonly string gameModeHeaderWave = "Mode: wave";
    private float minutes { get; set; }
    private float seconds { get; set; }

    private void Update()
    {
        if (Object != null && _isTimerOn)
        {
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
            }
            else
            {
                TimeLeft = 0;
                _isTimerOn = false;
            }
        }
    }

    public static void OnTimerChanged(Changed<GameController> changed)
    {
        var gameController = changed.Behaviour;
        var currentTime = gameController.TimeLeft + 1;

        gameController.minutes = Mathf.FloorToInt(currentTime / 60);
        gameController.seconds = Mathf.FloorToInt(currentTime % 60);

        gameController.TimerText.text = string.Format("{0:00} : {1:00}", gameController.minutes, gameController.seconds);
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
            _gameStarted = true;
            _isTimerOn = true;
            OnGameStarted?.Invoke(this, EventArgs.Empty);
            RPC_UpdateGameModeHeader(gameModeHeaderWave);
        }
    }

    [Rpc]
    private void RPC_UpdateGameModeHeader(string header, RpcInfo info = default)
    {
        GameModeHeader.text = header;
    }

    public void EndGame()
    {
        Runner.Shutdown();
    }

    private void SetTimer(float seconds)
    {
        TimeLeft = seconds;
        _isTimerOn = true;
    }

    private void StartRest()
    {
        _enemySpawner.StopSpawning();
        _collectablesSpawner.StopSpawning();
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
                case GameState.Rest: ChooseNextRound(); RPC_UpdateGameModeHeader(gameModeHeaderWave); break;
                case GameState.Round: StartRest(); SetWaveAsCompleted(); RPC_UpdateGameModeHeader(gameModeHeaderRest); break;
            }
        }
    }

    private void SetWaveAsCompleted()
    {
        waveCompletion[_currentWaveId] = true;
    }

    public void FinishGame()
    {
        OnGameFinished?.Invoke(this, EventArgs.Empty);
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
            _collectablesSpawner.StopSpawning();
            _enemySpawner.StopSpawning();
            FinishGame();
        }
    }
}
