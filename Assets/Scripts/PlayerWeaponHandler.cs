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
    private Transform _aimTransform;
    private int _multipleShootAmount = 3;

    public event EventHandler OnWeaponAssigned;

    [Rpc]
    public void RPC_ShowWeapon(RpcInfo info = default)
    {
        _weaponSpriteRenderer.sprite = _weaponSO.sprite;
    }

    public void AssignWeapon(WeaponScriptableObject weaponSO)
    {
        _weaponSO = weaponSO;
        _gunPoint.transform.localPosition = new Vector3(_weaponSO.shootStartPoints.X, _weaponSO.shootStartPoints.Y, 0);
        OnWeaponAssigned?.Invoke(this, EventArgs.Empty);
    }

    private void Awake()
    {
        _aimTransform = GetComponent<Transform>();
    }

    [Rpc]
    public void RPC_Aim(NetworkInputData inputData)
    {
        Debug.Log("AIMING ");
        Vector2 shootDirection = inputData.shootDirection;
        float aimAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        _aimTransform.eulerAngles = new Vector3(0, 0, aimAngle);

        Vector3 aimLocalScale = Vector3.one;
        if (aimAngle > 90 || aimAngle < -90)
        {
            aimLocalScale.y = -1f;
        }
        else
        {
            aimLocalScale.y = +1f;
        }
        _aimTransform.localScale = aimLocalScale;
    }

    [Rpc]
    public void RPC_Shoot(int ammoAmount)
    {
        if (ammoAmount > 0)
        {
            if (_weaponSO.canFireMultiple)
            {
                FireBullet(_multipleShootAmount);
            }
            else
            {
                FireBullet(1);
            }
            ammoAmount -= 1;
        }
    }

    private void FireBullet(int bulletAmount)
    {
        for (int i = 0; i < bulletAmount; i++)
        {
            Vector3 startPosition = new Vector3(_gunPoint.transform.position.x + i, _gunPoint.transform.position.y + i, _gunPoint.transform.position.z);
            var bullet = Runner.Spawn(_bulletPrefab, startPosition, Quaternion.identity, Object.InputAuthority);
            bullet.GetComponent<Bullet>().AssignData(_weaponSO.bulletSpeed, _weaponSO.damage);
        }
    }

}