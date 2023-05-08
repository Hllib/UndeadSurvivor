using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInputHandler : MonoBehaviour
{
    private NetworkPlayer _player;
    [SerializeField] private VariableJoystick moveJoystick;
    [SerializeField] private VariableJoystick shootJoystick;
    private float _speed = 5f;

    private void Awake()
    {
        _player = GetComponent<NetworkPlayer>();
        _player.OnUIInstantiated += FindJoysticks;
    }

    private void FindJoysticks(object sender, EventArgs e)
    {
        moveJoystick = GameObject.FindGameObjectWithTag("MoveJoystick").GetComponent<VariableJoystick>();
        shootJoystick = GameObject.FindGameObjectWithTag("ShootJoystick").GetComponent<VariableJoystick>();
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData data = new NetworkInputData();
        if (moveJoystick != null && shootJoystick != null)
        {
            data.moveDirection = Vector2.up * moveJoystick.Vertical * _speed + Vector2.right * moveJoystick.Horizontal * _speed;
            data.shootDirection = Vector2.up * shootJoystick.Vertical + Vector2.right * shootJoystick.Horizontal;
        }

        return data;
    }
}
