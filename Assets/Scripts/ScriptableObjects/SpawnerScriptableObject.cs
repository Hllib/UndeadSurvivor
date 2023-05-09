using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spawner", menuName = "ScriptableObjects/New Spawner")]
public class SpawnerScriptableObject : ScriptableObject
{
    public NetworkPrefabRef[] spawnObjects;
    public float spawnDelay;
}
