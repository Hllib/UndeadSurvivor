using Fusion;
using System.Linq;
using UnityEngine;

public class BombButton : NetworkBehaviour
{
    [SerializeField] private NetworkPlayer _player;

    public void DropBomb()
    {
        if (_player.bombAmount > 0)
        {
            _player.RPC_DropBomb();
        }
    }
}
