using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthKit : NetworkBehaviour
{
    private int _healthToAdd = 3;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<NetworkPlayer>();
        if (player != null)
        {
            player.UpdateHealth(_healthToAdd, true);
            Runner.Despawn(Object);
        }
    }
}
