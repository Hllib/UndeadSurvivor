using Collectables;

public class BombCollectable : TriggerableByPlayer
{
    protected override void ActionOnTrigger(NetworkPlayer player)
    {
        base.ActionOnTrigger(player);         
        
        player.AddBomb();
        Runner.Despawn(Object);
    }
}
