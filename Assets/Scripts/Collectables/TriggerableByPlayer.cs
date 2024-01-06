using System;
using Fusion;
using UnityEngine;

namespace Collectables
{
    public class TriggerableByPlayer : NetworkBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out NetworkPlayer player))
            {
                ActionOnTrigger(player);
            }   
        }

        protected virtual void ActionOnTrigger(NetworkPlayer player)
        {
            
        }
    }
}