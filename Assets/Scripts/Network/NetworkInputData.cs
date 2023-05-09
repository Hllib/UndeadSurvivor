using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 MoveDirection;
    public Vector2 ShootDirection;
    public bool CanShoot;
}
