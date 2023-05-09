using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _ammoText;
    private NetworkPlayer _player;
    private PlayerWeaponHandler _playerWeaponHandler;

    private void Awake()
    {
        _player = GetComponent<NetworkPlayer>();
        _playerWeaponHandler = GetComponent<PlayerWeaponHandler>();

        _player.OnUIInstantiated += FindHUD;
        _player.OnHealthChanged += UpdateHealthUI;
        _playerWeaponHandler.OnAmmoChanged += UpdateAmmoUI;
    }

    private void FindHUD(object sender, EventArgs e)
    {
        _healthText = GameObject.FindGameObjectWithTag("HealthUI").GetComponent<TextMeshProUGUI>();
        _ammoText = GameObject.FindGameObjectWithTag("AmmoUI").GetComponent<TextMeshProUGUI>();
    }

    private void UpdateHealthUI(int healthAmount)
    {
        if (_healthText != null)
        {
            _healthText.text = healthAmount.ToString();
        }
    }

    private void UpdateAmmoUI(int ammoAmoutn)
    {
        if (_ammoText != null)
        {
            _ammoText.text = ammoAmoutn.ToString();
        }
    }
}
