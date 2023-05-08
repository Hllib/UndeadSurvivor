using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAssigner : NetworkBehaviour
{
    [SerializeField] private NetworkRunnerHandler _networkRunner;
    [SerializeField] private WeaponScriptableObject[] _weaponVariantsSO;
    [SerializeField] private GameController _gameController;

    private void Awake()
    {
        _gameController.OnGameStarted += AssignWeaponsToPlayers;
    }

    private void AssignWeaponsToPlayers(object sender, EventArgs e)
    {
        List<int> usedWeapons = new List<int>();
        foreach (var playerObj in _networkRunner.spawnedCharacters.Values)
        {
            int randomIndex = 0;
            do
            {
                randomIndex = UnityEngine.Random.Range(0, _weaponVariantsSO.Length);
            } while (usedWeapons.Contains(randomIndex));
            usedWeapons.Add(randomIndex.GetHashCode());

            playerObj.GetComponent<PlayerWeaponHandler>().AssignWeapon(_weaponVariantsSO[randomIndex]);
        }
    }
}
