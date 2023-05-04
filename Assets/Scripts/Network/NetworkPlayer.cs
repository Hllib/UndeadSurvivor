using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cinemachine;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    private NetworkRigidbody2D _rb;
    CinemachineVirtualCamera _camera;
    private GameController _gameControlller;
    [SerializeField] private GameObject _bulletPrefab;

    private int _ammoAmount;
    private int _health;

    public void AddAmmo(int ammoSurplus)
    {
        _ammoAmount += ammoSurplus;
        UIManager.Instance.UpdateAmmo(_ammoAmount);
    }

    public void UpdateHealth(int unitsToRemove)
    {
        _health -= unitsToRemove;
        UIManager.Instance.UpdateHealth(_health);
    }

    public void UpdateHealth(int unitsToAdd, bool isHealing)
    {
        _health += unitsToAdd;
        UIManager.Instance.UpdateHealth(_health);
    }

    private void Awake()
    {
        _rb ??= GetComponent<NetworkRigidbody2D>();
    }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            _rb ??= GetComponent<NetworkRigidbody2D>();
            Debug.Log("Spawned own player");
            _camera = GameObject.FindGameObjectWithTag("VCam").GetComponent<CinemachineVirtualCamera>();
            _camera.Follow = this.transform;
        }
        else
        {
            Debug.Log("Spawned another player");
        }

        if(Object.HasStateAuthority) 
        {
            _gameControlller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }
    }

    private bool _hasGameStarted;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _rb.Rigidbody.velocity = data.direction;
        }

        if(Input.GetKey(KeyCode.R) && Object.HasStateAuthority && !_hasGameStarted)
        {
            _hasGameStarted = true;
            _gameControlller.StartGame();
        }

        if(Input.GetKeyDown(KeyCode.Space) && Object.HasInputAuthority)
        {
            //Runner.Spawn();
        }
    }
}
