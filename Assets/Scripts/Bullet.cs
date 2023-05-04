using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public override void FixedUpdateNetwork()
    {
        transform.Translate(Vector2.down * 1f * Time.deltaTime);
    }
}
