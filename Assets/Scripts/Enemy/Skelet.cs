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
        currentAI = enemyScriptableObject;

        speed = currentAI.speed;
        Health = currentAI.health;

        attackRadius = currentAI.attackRadius;
        attackRate = currentAI.attackRate;
        damage = currentAI.damage;
    }

    protected override void Attack()
    {
        Vector2 shootDirection = (player.transform.position - transform.position).normalized;
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
        float distance = Vector3.Distance(this.transform.localPosition, player.transform.localPosition);
        if (distance <= this.attackRadius)
        {
            speed = 0;
        }
        else
        {
            speed = currentAI.speed;
        }
    }
}
