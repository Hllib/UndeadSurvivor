using Fusion;
using UnityEngine;

public class BombCollectable : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.GetComponent<NetworkPlayer>();
        if (player != null)
        {
            player.AddBomb();
            Runner.Despawn(Object);
        }
    }
}
