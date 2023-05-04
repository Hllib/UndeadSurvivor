using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private int _damage = 10;

    public override void FixedUpdateNetwork()
    {
        transform.Translate(Vector2.right * 3f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damagedObject = collision.GetComponent<IDamageable>();

        if (damagedObject != null)
        {
            damagedObject.Damage(_damage);
            Runner.Despawn(Object);
        }
    }

}
