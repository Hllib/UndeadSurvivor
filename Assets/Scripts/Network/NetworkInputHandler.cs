using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkInputHandler : MonoBehaviour
{
    private float _speed = 5.0f;
    //public delegate void MovementSender(float hor, float ver);
    //public event MovementSender onPlayerInput;

    public NetworkInputData GetNetworkInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.direction = new Vector2(horizontalInput * _speed, verticalInput * _speed);
        //onPlayerInput?.Invoke(horizontalInput, verticalInput);

        return networkInputData;
    }
}
