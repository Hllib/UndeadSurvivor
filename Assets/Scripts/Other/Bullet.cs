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
    NetworkPlayer _player;

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
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
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

    private void SetPlayer(NetworkPlayer player)
    {
        _player = player;
    }

    public void AssignData(float speed, int damage, Vector3 direction, NetworkPlayer player)
    {
        SetSpeed(speed);
        SetDamage(damage);
        SetDirection(direction);
        SetPlayer(player);
        _hasValuesAssigned = true;
    }

    private void UpdatePlayerScore(int damageSurplus, bool hasKilled)
    {
        int killCountSurplus = hasKilled ? 1 : 0;
        _player.UpdateScore(damageSurplus, killCountSurplus);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_hasValuesAssigned)
        {
            var damagedObject = collision.GetComponent<IDamageable>();

            if (damagedObject != null)
            {
                if (this != null)
                {
                    StopAllCoroutines();
                    if (_player != null)
                    {
                        UpdatePlayerScore(_damage, damagedObject.Health <= _damage);
                    }
                    damagedObject.Damage(_damage);
                    if (Object != null)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

}
