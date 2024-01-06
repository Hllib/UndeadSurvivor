using Collectables;
using UnityEngine;

public class HealthKit : TriggerableByPlayer
{
    [SerializeField] private int _healthToAdd = 3;

    protected override void ActionOnTrigger(NetworkPlayer player)
    {
        base.ActionOnTrigger(player);
        
        player.UpdateHealth(_healthToAdd, true);
        Runner.Despawn(Object);
    }
}
