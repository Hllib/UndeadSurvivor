using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [SerializeField] private WeaponScriptableObject _weaponSO;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<NetworkPlayer>();
        if (player != null)
        {
            var weaponHandler = player.GetComponent<PlayerWeaponHandler>();
            weaponHandler.AssignWeapon(_weaponSO);
            weaponHandler.RPC_ShowWeapon();
            Runner.Despawn(Object);
        }
    }
}
