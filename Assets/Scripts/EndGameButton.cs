using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EndGameButton : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameController _gameController;

    public void OnPointerClick(PointerEventData eventData)
    {
        _gameController.EndGame();
        Runner.Despawn(Object);
    }
}
