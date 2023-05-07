using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUIHandler : NetworkBehaviour
{
    private TextMeshProUGUI _healthText;
    private TextMeshProUGUI _ammoText;
    private NetworkPlayer _player;
    private PlayerWeaponHandler _playerWeaponHandler;

    private void Awake()
    {
        _healthText = GameObject.FindGameObjectWithTag("HealthUI").GetComponent<TextMeshProUGUI>();
        _ammoText = GameObject.FindGameObjectWithTag("AmmoUI").GetComponent<TextMeshProUGUI>();
        _player = GetComponent<NetworkPlayer>();
        _playerWeaponHandler = GetComponent<PlayerWeaponHandler>();

        _player.OnHealthChanged += UpdateHealthUI;
        _playerWeaponHandler.OnAmmoChanged += UpdateAmmoUI;
    }

    private void UpdateHealthUI(int healthAmount)
    {
        _healthText.text = healthAmount.ToString();
    }

    private void UpdateAmmoUI(int ammoAmoutn)
    {
        _ammoText.text = ammoAmoutn.ToString();
    }
}
