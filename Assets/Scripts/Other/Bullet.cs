using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private int _damage;
    private float _speed;
    private bool _hasValuesAssigned;
    private Vector3 _direction;

    public override void FixedUpdateNetwork()
    {
        if (_hasValuesAssigned)
        {
            transform.Translate(_direction * _speed * Time.deltaTime);
        }
    }

    public override void Spawned()
    {
        StartCoroutine(DestroyBullet());
    }

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(2f);
        Runner.Despawn(Object);
    }

    private void SetSpeed(float speed)
    {
        _speed = speed;
    }

    private void SetDamage(int damage)
    {
        _damage = damage;
    }

    private void SetDirection(Vector3 direction)
    {
        _direction = direction;
    }

    public void AssignData(float speed, int damage, Vector3 direction)
    {
        SetSpeed(speed);
        SetDamage(damage);
        SetDirection(direction);
        _hasValuesAssigned = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damagedObject = collision.GetComponent<IDamageable>();

        if (damagedObject != null)
        {
            StopAllCoroutines();
            damagedObject.Damage(_damage);
            Runner.Despawn(Object);
        }
    }

}
