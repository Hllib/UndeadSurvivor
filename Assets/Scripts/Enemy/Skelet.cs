using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skelet : Enemy, IDamageable
{
    public int Health { get; set; }
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
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
        Debug.Log("Skelet shooting");
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
