using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    private NetworkRigidbody2D _rb;
    public static NetworkPlayer Local { get; set; }
    CinemachineVirtualCamera _camera;

    [SerializeField] private Enemy _enemyPerfab;

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
            _camera = GameObject.FindGameObjectWithTag("VCam").GetComponent<CinemachineVirtualCamera>();
            _camera.Follow = this.transform;
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
    bool hasSpawned = false;
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _rb.Rigidbody.velocity = data.direction;
        }

        if(Input.GetKey(KeyCode.R) && !hasSpawned && Object.HasStateAuthority)
        {
            hasSpawned = true;
            Runner.Spawn(_enemyPerfab, transform.position, Quaternion.identity);
        }
    }
}
