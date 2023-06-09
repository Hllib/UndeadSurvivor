using Fusion;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeaponHandler : NetworkBehaviour
{
    private WeaponScriptableObject _weaponSO;
    [SerializeField] private SpriteRenderer _weaponSpriteRenderer;
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _gunPoint;
    [SerializeField] private Transform _aimTransform;
    private int _multipleShootAmount = 3;
    private int _ammoAmount;

    private float _canShoot = 0f;
    private float _shootRate = 1.5f;
    private bool _canWeaponFireMultiple;

    public delegate void AmmoSender(int ammoAmount);
    public event AmmoSender OnAmmoChanged;

    public void UpdateAmmo(int ammoSurplus, bool toAdd)
    {
        switch (toAdd)
        {
            case true: _ammoAmount += ammoSurplus; break;
            default: _ammoAmount -= ammoSurplus; break;
        }
        RPC_UpdateAmmoAmount(_ammoAmount);
        OnAmmoChanged?.Invoke(_ammoAmount);
    }

    [Rpc]
    private void RPC_UpdateAmmoAmount(int ammoAmount, RpcInfo info = default)
    {
        _ammoAmount = ammoAmount;
    }

    private void Awake()
    {
        this.GetComponent<NetworkPlayer>().OnPlayerDead += OnPlayerDead;
    }

    private void OnPlayerDead(object sender, EventArgs e)
    {
        RPC_DisablePlayerWeapon();
    }

    [Rpc]
    private void RPC_DisablePlayerWeapon()
    {
        _weaponSpriteRenderer.gameObject.SetActive(false);
    }

    [Rpc]
    private void RPC_ShowWeapon(string spriteName, RpcInfo info = default)
    {
        _weaponSpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Weapons/" + spriteName);
    }

    [Rpc]
    private void RPC_SetFireMultiple(bool state, RpcInfo info = default)
    {
        _canWeaponFireMultiple = state;
    }

    [Rpc]
    private void RPC_UpdateShootRate(float shootRate, RpcInfo info = default)
    {
        _shootRate = shootRate;
    }

    public void AssignWeapon(WeaponScriptableObject weaponSO)
    {
        _weaponSO = weaponSO;
        _gunPoint.transform.localPosition = new Vector3(_weaponSO.shootStartPoints.X, _weaponSO.shootStartPoints.Y, 0);
        _weaponSpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Weapons/" + _weaponSO.title);
        RPC_SetFireMultiple(_weaponSO.canFireMultiple);
        RPC_ShowWeapon(_weaponSO.title);
        RPC_UpdateShootRate(_weaponSO.shootRate);
    }

    [Rpc]
    public void RPC_Aim(NetworkInputData inputData)
    {
        Vector2 shootDirection = inputData.ShootDirection;
        float aimAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        _aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);

        Vector3 aimLocalScale = Vector3.one;
        aimLocalScale.y = (aimAngle > 90 || aimAngle < -90) ? -1f : 1f;
        _aimTransform.localScale = aimLocalScale;

        if (_weaponSO != null && inputData.CanShoot)
        {
            if (Time.time > _canShoot && _ammoAmount > 0)
            {
                RPC_Shoot();
                _canShoot = Time.time + _shootRate;
            }
        }
    }

    [Rpc]
    public void RPC_Shoot()
    {
        if (_canWeaponFireMultiple)
        {
            FireBullet(_multipleShootAmount);
        }
        else
        {
            FireBullet(1);
        }
        UpdateAmmo(1, false);
    }

    private void FireBullet(int bulletAmount)
    {
        if (Object.HasStateAuthority)
        {
            float offset = _canWeaponFireMultiple ? -0.3f : 0f;
            for (int i = 0; i < bulletAmount; i++)
            {
                Vector3 startPosition = new Vector3(_gunPoint.transform.position.x, _gunPoint.transform.position.y + offset, _gunPoint.transform.position.z);
                var bullet = Runner.Spawn(_bulletPrefab, startPosition, Quaternion.identity, Object.InputAuthority);
                bullet.GetComponent<Bullet>().AssignData(_weaponSO.bulletSpeed, _weaponSO.damage, _gunPoint.transform.right, GetComponent<NetworkPlayer>());
                offset += 0.3f;
            }
        }
    }
}