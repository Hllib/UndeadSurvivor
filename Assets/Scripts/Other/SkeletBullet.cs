using Fusion;
using System.Collections;
using UnityEngine;

public class SkeletBullet : NetworkBehaviour
{
    private int _damage;
    private float _speed = 10f;
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
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    public void AssignData(int damage, Vector3 direction)
    {
        _damage = damage;
        _direction = direction;
        _hasValuesAssigned = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var damagedObject = collision.GetComponent<NetworkPlayer>();

        if (damagedObject != null)
        {
            if (this != null)
            {
                StopAllCoroutines();
                damagedObject.Damage(_damage);
                Destroy(gameObject);
            }
        }
    }
}
