using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : NetworkBehaviour
{
    private int _ammoInBox = 15;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<NetworkPlayer>();
        if (player != null)
        {
            player.GetComponentInChildren<PlayerWeaponHandler>().UpdateAmmo(_ammoInBox, true);
            Runner.Despawn(Object);
        }
    }
}
