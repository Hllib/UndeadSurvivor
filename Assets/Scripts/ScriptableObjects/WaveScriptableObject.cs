using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveSettings", menuName = "ScriptableObjects/New Wave Settings")]
public class WaveScriptableObject : ScriptableObject
{
    public float wave1Duration;
    public float wave2Duration;
    public float wave3Duration;
    public float restTime;
}
