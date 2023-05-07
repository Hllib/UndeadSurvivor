using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;
using System;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft, IDamageable
{
    private NetworkRigidbody2D _rb;
    CinemachineVirtualCamera _camera;

    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private NetworkPlayerAnimator _networkAnimator;
    [SerializeField] private PlayerWeaponHandler _weaponHandler;

    private int _initialHealth = 10;
    public int Health { get; set; }

    public int damageDone;
    public int enemiesKilled;
    private int _bombAmount;
    public string playerName;

    public delegate void HealthSender(int healthAmount);
    public event HealthSender OnHealthChanged;

    public void AddBomb()
    {
        _bombAmount += 1;
    }

    public void UpdateScore(int damageDoneSurplus, int enemiesKilledSurplus)
    {
        damageDone += damageDoneSurplus;
        enemiesKilled += enemiesKilledSurplus;
    }

    public void UpdateHealth(int healthSurplus, bool toAdd)
    {
        switch (toAdd)
        {
            case true: Health += healthSurplus; break;
            default: Health -= healthSurplus; break;
        }
        OnHealthChanged?.Invoke(Health);
    }

    private void Awake()
    {
        _rb ??= GetComponent<NetworkRigidbody2D>();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            _rb ??= GetComponent<NetworkRigidbody2D>();
            _camera = GameObject.FindGameObjectWithTag("VCam").GetComponent<CinemachineVirtualCamera>();
            _camera.Follow = this.transform;
            Health = _initialHealth;
            UIManager.Instance.UpdateHealth(Health);
            string name = Object.HasStateAuthority ? "Host" : "Client";
            RPC_UpdateName(name);
        }
    }

    [Rpc]
    private void RPC_UpdateName(string name, RpcInfo info = default)
    {
        playerName = name;
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
        if (GetInput(out NetworkInputData data))
        {
            _rb.Rigidbody.velocity = data.moveDirection;
            _networkAnimator.RPC_ChooseAnimation(data);
            _weaponHandler.RPC_Aim(data);

            if (data.canDropBomb && _bombAmount > 0)
            {
                RPC_DropBomb();
            }
        }
    }

    public void Damage(int damage)
    {
        UpdateHealth(damage, false);
        if (Health <= 0)
        {
            UIManager.Instance.UpdateHealth(Health);
            Die();
        }
    }

    public event EventHandler OnPlayerDead;

    private void Die()
    {

    }
}
