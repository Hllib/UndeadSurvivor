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
            player.AssignWeapon(_weaponSO);
            player.RPC_ShowWeapon();
            Runner.Despawn(Object);
        }
    }
}
