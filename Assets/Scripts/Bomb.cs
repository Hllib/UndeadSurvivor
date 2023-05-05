using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : NetworkBehaviour
{
    [SerializeField] private CircleCollider2D _circleCollider;
    private bool _canExplode;
    private int _explosionDamage = 10;

    private void DealDamageAround()
    {
        _canExplode = true;
        _circleCollider.radius *= 7;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_canExplode)
        {
            IDamageable hit = collision.GetComponent<IDamageable>();

            if (hit != null)
            {
                Debug.Log(collision.name);
                hit.Damage(_explosionDamage);
            }
        }
    }

    public override void Spawned()
    {
        StartCoroutine(ExplodeCountdown());
    }

    IEnumerator ExplodeCountdown()
    {
        yield return new WaitForSeconds(3f);
        DealDamageAround();
        yield return new WaitForSeconds(0.5f);
        Runner.Despawn(Object);
    }
}