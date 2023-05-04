using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skelet : Enemy, IDamageable
{
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    EnemyScriptableObject currentAI; 

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected override void SetInitialSettings()
    {
        currentAI = enemyScriptableObject;

        speed = currentAI.speed;
        health = currentAI.health;

        attackRadius = currentAI.attackRadius;
        attackRate = currentAI.attackRate;
        damage = currentAI.damage;
    }

    protected override void Attack()
    {
        Debug.Log("DEALING DAMAGE TO PLAYER");
        player.Damage(currentAI.damage);
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
