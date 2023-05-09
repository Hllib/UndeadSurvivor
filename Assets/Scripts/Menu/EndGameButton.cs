using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndGameButton : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameController _gameController;
    private NetworkRunnerHandler _networkRunnerHandler;

    public override void Spawned()
    {
        _networkRunnerHandler = Runner.GetComponent<NetworkRunnerHandler>();    
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
            _gameController.EndGame();
        }
    }
}
