using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy, IDamageable
{    
    EnemyScriptableObject currentAI;

    protected override void SetInitialSettings()
    {
        currentAI = enemyScriptableObject;

        speed = currentAI.speed;
        attackRadius = currentAI.attackRadius;
        attackRate = currentAI.attackRate;
        damage = currentAI.damage;  
        health = currentAI.health;
    }

    protected override void Attack()
    {
        player.Damage(currentAI.damage);
    }

    public void Damage(int damage)
    {
        Debug.Log("Taking damage");
        health -= damage;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }
}

