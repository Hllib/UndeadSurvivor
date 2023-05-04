using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/New Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    public string title;
    public Sprite sprite;
    public float damage;
    public bool canFireMultiple;
    public float bulletSpeed;

    [Serializable]
    public struct ShootStartPoints
    {
        public float X;
        public float Y;
    }

    public ShootStartPoints shootStartPoints;
}
