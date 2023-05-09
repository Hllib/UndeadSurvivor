using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _networkRunner;
    private NetworkInputHandler _playerInputHandler;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    public Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    [SerializeField] private GameObject _coverUI;
    [SerializeField] private GameObject[] _spawnPoints;

    private void Start()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        var task = InitializeNetworkRunner(_networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
        yield return new WaitUntil(() => task.IsCompleted);
        _coverUI.SetActive(false);
    }

    private void Awake()
    {
        _networkRunner = GetComponent<NetworkRunner>();
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneObjectProvider = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneObjectProvider == null)
        {
            sceneObjectProvider = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            //SessionName = "UndeadSurvivor", 
            Initialized = initialized,
            SceneManager = sceneObjectProvider
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, GetSpawnPos(), Quaternion.identity, player);
            spawnedCharacters.Add(player, networkPlayerObject);
        }
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

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (_playerInputHandler == null && NetworkPlayer.Local != null)
        {
            _playerInputHandler = NetworkPlayer.Local.GetComponent<NetworkInputHandler>();
        }

        if (_playerInputHandler != null)
        {
            var inputData = _playerInputHandler.GetNetworkInput();
            inputData.canShoot = (inputData.shootDirection.x != 0 || inputData.shootDirection.y != 0) ? true : false;
            input.Set(inputData);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        runner.Shutdown();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }
}
