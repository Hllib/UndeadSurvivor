using Collectables;
using UnityEngine;

public class AmmoBox : TriggerableByPlayer
{
    [SerializeField] private int _ammoInBox = 15;

    protected override void ActionOnTrigger(NetworkPlayer player)
    {
        base.ActionOnTrigger(player);
        
        player.WeaponHandler.UpdateAmmo(_ammoInBox, true);
        Runner.Despawn(Object);
    }
}
