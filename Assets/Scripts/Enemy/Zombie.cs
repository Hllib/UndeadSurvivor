using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy, IDamageable
{    
    public int Health { get; set; }
    EnemyScriptableObject currentAI;

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
        Debug.Log("Damaging player");
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

