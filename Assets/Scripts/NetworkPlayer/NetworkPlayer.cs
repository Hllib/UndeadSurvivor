using UnityEngine;
using Fusion;
using Cinemachine;
using System;
using System.Linq;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft, IDamageable
{
    public delegate void HealthSender(int healthAmount);
    public event HealthSender OnHealthChanged;
    public event EventHandler OnUIInstantiated;
    public event EventHandler OnPlayerDead;

    private int _damageDone;
    private int _enemiesKilled;
    private int _bombAmount;
    private string _playerName;

    public int DamageDone => _damageDone;
    public int EnemiesKilled => _enemiesKilled;
    public int BombAmount => _bombAmount;
    public string PlayerName => _playerName;

    public static NetworkPlayer Local { get; set; }
    public int Health { get; set; }
    public bool IsDead { get; private set; }

    private int _initialHealth = 10;
    private NetworkRigidbody2D _rb;
    private CinemachineVirtualCamera _camera;

    [SerializeField] private GameObject _playerCanvas;
    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private NetworkPlayerAnimator _networkAnimator;
    [SerializeField] private PlayerWeaponHandler _weaponHandler;
    private GameObject _canvas;

    public void AddBomb()
    {
        _bombAmount += 1;
    }

    public void UpdateScore(int damageDoneSurplus, int enemiesKilledSurplus)
    {
        _damageDone += damageDoneSurplus;
        _enemiesKilled += enemiesKilledSurplus;
    }

    public void UpdateHealth(int healthSurplus, bool toAdd)
    {
        switch (toAdd)
        {
            case true: Health += healthSurplus; break;
            default: Health -= healthSurplus; break;
        }
        RPC_UpdateHealth(Health);
        OnHealthChanged?.Invoke(Health);
    }

    [Rpc]
    private void RPC_UpdateHealth(int healthAmount, RpcInfo info = default)
    {
        Health = healthAmount;
    }

    private void Awake()
    {
        _rb ??= GetComponent<NetworkRigidbody2D>();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            _camera = GameObject.FindGameObjectWithTag("VCam").GetComponent<CinemachineVirtualCamera>();
            _camera.Follow = this.transform;
            string name = Object.HasStateAuthority ? "Host" : "Client";
            RPC_UpdateName(name);
            _canvas = Instantiate(_playerCanvas, this.transform);
            OnUIInstantiated?.Invoke(this, EventArgs.Empty);
        }
        RPC_UpdateHealth(_initialHealth);
    }

    private void TurnOnSpectator()
    {
        if(HasInputAuthority)
        {
            _canvas.SetActive(false);
            CinemachineVirtualCamera spectatorCamera = GameObject.FindGameObjectWithTag("SpectatorCamera").GetComponent<CinemachineVirtualCamera>();
            _camera.Follow = spectatorCamera.Follow;
            _camera.transform.position = spectatorCamera.transform.position;
            _camera.transform.rotation = spectatorCamera.transform.rotation;
        }
    }

    [Rpc]
    private void RPC_UpdateName(string name, RpcInfo info = default)
    {
        _playerName = name;
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    [Rpc]
    public void RPC_DropBomb(RpcInfo info = default)
    {
        _bombAmount -= 1;
        Runner.Spawn(_bombPrefab, transform.position, Quaternion.identity, Object.InputAuthority);
    }

    public override void FixedUpdateNetwork()
    {
        if (IsDead)
        {
            return;
        }
        if (GetInput(out NetworkInputData data))
        {
            _rb.Rigidbody.velocity = data.MoveDirection;
            _networkAnimator.RPC_ChooseAnimation(data);
            _weaponHandler.RPC_Aim(data);
        }
    }

    public void Damage(int damage)
    {
        if (!IsDead)
        {
            UpdateHealth(damage, false);
            if (Health <= 0)
            {
                RPC_Die();
                OnPlayerDead?.Invoke(this, EventArgs.Empty);
                TurnOnSpectator();
            }
        }
    }

    [Rpc]
    private void RPC_Die()
    {
        IsDead = true;
    }
}
