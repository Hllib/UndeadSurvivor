using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInputHandler : MonoBehaviour
{
    [SerializeField] private VariableJoystick moveJoystick;
    [SerializeField] private VariableJoystick shootJoystick;
    private float _speed = 5f;

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData data = new NetworkInputData();
        data.moveDirection = Vector2.up * moveJoystick.Vertical * _speed + Vector2.right * moveJoystick.Horizontal * _speed;
        data.shootDirection = Vector2.up * shootJoystick.Vertical + Vector2.right * shootJoystick.Horizontal;
        return data;
        //PC INPUT
        //float horizontalInput = Input.GetAxisRaw("Horizontal");
        //float verticalInput = Input.GetAxisRaw("Vertical");
        //NetworkInputData networkInputData = new NetworkInputData();
        //networkInputData.moveDirection = new Vector2(horizontalInput * _speed, verticalInput * _speed);
        //return networkInputData;
    }
}
