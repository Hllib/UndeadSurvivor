using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy, IDamageable
{    
    EnemyScriptableObject currentAI;
    public int Health { get; set; }

    protected override void SetInitialSettings()
    {
        currentAI = EnemyScriptableObject;

        Speed = currentAI.speed;
        AttackRadius = currentAI.attackRadius;
        AttackRate = currentAI.attackRate;
        base.DamageDone = currentAI.damage;  
        Health = currentAI.health;
    }

    protected override void Attack()
    {
        Player.Damage(currentAI.damage);
    }

    public void Damage(int damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

