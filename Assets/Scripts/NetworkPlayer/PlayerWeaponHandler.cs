using Fusion;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
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

    private float canShoot = 0f;
    private float shootRate = 1.5f;

    public delegate void AmmoSender(int ammoAmount);
    public event AmmoSender OnAmmoChanged;

    public void UpdateAmmo(int ammoSurplus, bool toAdd)
    {
        switch (toAdd)
        {
            case true: _ammoAmount += ammoSurplus; break;
            default: _ammoAmount -= ammoSurplus; break;
        }
        OnAmmoChanged?.Invoke(_ammoAmount);
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
    public void RPC_ShowWeapon()
    {
        _weaponSpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Weapons/" + _weaponSO.title);
    }

    public void AssignWeapon(WeaponScriptableObject weaponSO)
    {
        _weaponSO = weaponSO;
        _gunPoint.transform.localPosition = new Vector3(_weaponSO.shootStartPoints.X, _weaponSO.shootStartPoints.Y, 0);
    }

    [Rpc]
    public void RPC_Aim(NetworkInputData inputData)
    {
        Vector2 shootDirection = inputData.shootDirection;
        float aimAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        _aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);

        Vector3 aimLocalScale = Vector3.one;
        aimLocalScale.y = (aimAngle > 90 || aimAngle < -90) ? -1f : 1f;
        _aimTransform.localScale = aimLocalScale;

        if (_weaponSO != null && inputData.canShoot)
        {
            if (Time.time > canShoot && _ammoAmount > 0)
            {
                RPC_Shoot();
                canShoot = Time.time + shootRate;
            }
        }
    }

    [Rpc]
    public void RPC_Shoot()
    {
        if (_weaponSO.canFireMultiple)
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
            float offset = -0.3f;
            for (int i = 0; i < bulletAmount; i++)
            {
                Vector3 startPosition = new Vector3(_gunPoint.transform.position.x, _gunPoint.transform.position.y + offset, _gunPoint.transform.position.z);
                var bullet = Runner.Spawn(_bulletPrefab, startPosition, Quaternion.identity, Object.InputAuthority);
                Debug.Log(bullet.name);
                Debug.Log(GetComponent<NetworkPlayer>().playerName);
                bullet.GetComponent<Bullet>().AssignData(_weaponSO.bulletSpeed, _weaponSO.damage, _gunPoint.transform.right, GetComponent<NetworkPlayer>());
                offset += 0.3f;
            }
        }
    }
}