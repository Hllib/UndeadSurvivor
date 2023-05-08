using Fusion;
using System;
using System.Linq;
using UnityEngine;

public class BombButton : NetworkBehaviour
{
    private NetworkPlayer _player;

    private void Awake()
    {
        _player = GetComponentInParent<NetworkPlayer>();
    }

    public void DropBomb()
    {
        if (_player.bombAmount > 0)
        {
            _player.RPC_DropBomb();
        }
    }
}
