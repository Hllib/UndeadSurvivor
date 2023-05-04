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
    [SerializeField] private Bullet _bulletPrefab;
    private bool _hasGameStarted;
    private int _initialHealth = 10;

    private int _ammoAmount;
    private int _health;
    private WeaponScriptableObject _weaponSO;
    [SerializeField] private SpriteRenderer _weaponRenderer;
    private bool _hasWeaponAssigned;

    [SerializeField] private Transform _gunPoint;

    [Rpc]
    public void RPC_ShowWeapon(RpcInfo info = default)
    {
        _weaponRenderer.sprite = _weaponSO.sprite;
    }

    public void AssignWeapon(WeaponScriptableObject weaponSO)
    {
        _weaponSO = weaponSO;
        _gunPoint.transform.localPosition = new Vector3(_weaponSO.shootStartPoints.X, _weaponSO.shootStartPoints.Y, 0);
        _hasWeaponAssigned = true;
    }

    public void AddAmmo(int ammoSurplus)
    {
        _ammoAmount += ammoSurplus;
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
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            _rb ??= GetComponent<NetworkRigidbody2D>();
            Debug.Log("Spawned own player");
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
    public void RPC_Shoot(RpcInfo info = default)
    {
        Runner.Spawn(_bulletPrefab, _gunPoint.transform.position, Quaternion.identity, Object.InputAuthority);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _rb.Rigidbody.velocity = data.direction;
            if (_hasWeaponAssigned)
            {
                if (data.canShoot)
                {
                    RPC_Shoot();
                }
            }
        }

        if (Input.GetKey(KeyCode.R) && Object.HasStateAuthority && !_hasGameStarted)
        {
            _hasGameStarted = true;
            _gameControlller.StartGame();
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
        OnPlayerDead?.Invoke(this, EventArgs.Empty);
        Runner.Despawn(Object);
    }
}
