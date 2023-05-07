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

    public void AddBomb()
    {
        _bombAmount += 1;
    }

    public void UpdateScore(int damageDoneSurplus, int enemiesKilledSurplus)
    {
        damageDone += damageDoneSurplus;
        enemiesKilled += enemiesKilledSurplus;
    }

    //public void AddAmmo(int amountToAdd)
    //{
    //    _ammoAmount += amountToAdd;
    //    UIManager.Instance.UpdateAmmo(_ammoAmount);
    //}

    //private void MinusAmmo(object sender, EventArgs e)
    //{
    //    _ammoAmount -= 1;
    //}

    //public void UpdateAmmo(int ammoAmount)
    //{
    //    _ammoAmount = ammoAmount;
    //    UIManager.Instance.UpdateAmmo(_ammoAmount);
    //}

    public void UpdateHealth(int unitsToRemove)
    {
        Health -= unitsToRemove;
        UIManager.Instance.UpdateHealth(Health);
    }

    public void UpdateHealth(int unitsToAdd, bool isHealing)
    {
        Health += unitsToAdd;
        UIManager.Instance.UpdateHealth(Health);
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
            playerName = Object.HasStateAuthority ? "Host" : "Client";
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
