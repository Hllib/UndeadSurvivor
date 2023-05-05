using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInputHandler : MonoBehaviour
{
    private float _speed = 25.0f;
    public VariableJoystick joystick;

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData data = new NetworkInputData();
        data.direction = Vector2.up * joystick.Vertical + Vector2.right * joystick.Horizontal;
        Debug.Log("Data direction: " + data.direction);

        return data;
        //float horizontalInput = Input.GetAxisRaw("Horizontal");
        //float verticalInput = Input.GetAxisRaw("Vertical");
        //NetworkInputData networkInputData = new NetworkInputData();
        //networkInputData.direction = new Vector2(horizontalInput * _speed, verticalInput * _speed);
        //return networkInputData;
    }
}
