using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skelet : Enemy, IDamageable
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    public int Health { get; set; }

    private float canShoot = 0f;
    private float shootRate = 1.5f;

    EnemyScriptableObject currentAI;

    public void Damage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected override void SetInitialSettings()
    {
        currentAI = EnemyScriptableObject;

        Speed = currentAI.speed;
        Health = currentAI.health;

<<<<<<< HEAD
        AttackRadius = _currentAI.attackRadius;
        AttackRate = _currentAI.attackRate;
        base.DamageDone = _currentAI.damage;
=======
        AttackRadius = currentAI.attackRadius;
        AttackRate = currentAI.attackRate;
        base.Damage = currentAI.damage;
>>>>>>> parent of a5b0a4c (naming refactoring)
    }

    protected override void Attack()
    {
        Vector2 shootDirection = (Player.transform.position - transform.position).normalized;
        float aimAngle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        _firePoint.eulerAngles = new Vector3(0, 0, aimAngle);

        if(Time.time > canShoot)
        {
            if(Object.HasStateAuthority)
            {
                var bullet = Runner.Spawn(_bulletPrefab, _firePoint.transform.position, Quaternion.identity);
                bullet.GetComponent<SkeletBullet>().AssignData(currentAI.damage, _firePoint.transform.right);
                canShoot = Time.time + shootRate;
            }
        }
    }

    public override void CalculateMovement()
    {
        base.CalculateMovement();
        float distance = Vector3.Distance(this.transform.localPosition, Player.transform.localPosition);
        if (distance <= this.AttackRadius)
        {
            Speed = 0;
        }
        else
        {
            Speed = currentAI.speed;
        }
    }
}
