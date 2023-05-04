using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 direction;
    public bool canShoot;
    public bool canDropBomb;
}
