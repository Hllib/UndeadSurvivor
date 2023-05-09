using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy, IDamageable
{    
    private EnemyScriptableObject _currentAI;
    public int Health { get; set; }

    protected override void SetInitialSettings()
    {
        _currentAI = EnemyScriptableObject;

        Speed = _currentAI.speed;
        AttackRadius = _currentAI.attackRadius;
        AttackRate = _currentAI.attackRate;
        base.DamageDone = _currentAI.damage;  
        Health = _currentAI.health;
    }

    protected override void Attack()
    {
        Player.Damage(_currentAI.damage);
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

