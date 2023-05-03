using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    private NetworkRigidbody2D _rb;
    public static NetworkPlayer Local { get; set; }

    private void Awake()
    {
        _rb ??= GetComponent<NetworkRigidbody2D>();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            _rb ??= GetComponent<NetworkRigidbody2D>();
            Debug.Log("Spawned own player");
            //_camera = GameObject.FindGameObjectWithTag("VCam").GetComponent<CinemachineVirtualCamera>();
            //_camera.Follow = this.transform;
        }
        else
        {
            Debug.Log("Spawned another player");
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _rb.Rigidbody.velocity = data.direction;
        }
    }
}
