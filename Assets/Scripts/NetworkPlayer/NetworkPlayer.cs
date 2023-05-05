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
    private GameController _gameControlller;

    [SerializeField] private Bomb _bombPrefab;
    [SerializeField] private NetworkPlayerAnimator _networkAnimator;
    [SerializeField] private PlayerWeaponHandler _weaponHandler;

    private int _initialHealth = 10;
    private int _health;
    private int _ammoAmount = 15;
    private int _bombAmount;

    public void AddBomb()
    {
        _bombAmount += 1;
    }

    public void AddAmmo(int amountToAdd)
    {
        _ammoAmount += amountToAdd;
        UIManager.Instance.UpdateAmmo(_ammoAmount);
    }

    private void MinusAmmo(object sender, EventArgs e)
    {
        _ammoAmount -= 1;
    }

    public void UpdateAmmo(int ammoAmount)
    {
        _ammoAmount = ammoAmount;
        UIManager.Instance.UpdateAmmo(_ammoAmount);
    }

    public void UpdateHealth(int unitsToRemove)
    {
        _health -= unitsToRemove;
        UIManager.Instance.UpdateHealth(_health);
    }

    public void UpdateHealth(int unitsToAdd, bool isHealing)
    {
        _health += unitsToAdd;
        UIManager.Instance.UpdateHealth(_health);
    }

    private void Awake()
    {
        _rb ??= GetComponent<NetworkRigidbody2D>();
        _weaponHandler.OnShoot += MinusAmmo;
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            _rb ??= GetComponent<NetworkRigidbody2D>();
            _camera = GameObject.FindGameObjectWithTag("VCam").GetComponent<CinemachineVirtualCamera>();
            _camera.Follow = this.transform;
            _health = _initialHealth;
            UIManager.Instance.UpdateHealth(_health);
        }
        if (Object.HasStateAuthority)
        {
            _gameControlller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }
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
        UpdateHealth(damage);
        if (_health <= 0)
        {
            UIManager.Instance.UpdateHealth(_health);
            Die();
        }
    }

    public event EventHandler OnPlayerDead;

    private void Die()
    {
        Runner.Despawn(Object);
        //OnPlayerDead?.Invoke(this, EventArgs.Empty);
    }
}
