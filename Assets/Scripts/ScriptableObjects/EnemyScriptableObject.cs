using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/New Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public float speed;
    public int health;
    public float attackRate;
    public float attackRadius;
    public int damage;
}