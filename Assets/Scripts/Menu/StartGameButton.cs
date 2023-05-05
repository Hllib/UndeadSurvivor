using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartGameButton : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameController _gameController;

    public void OnPointerClick(PointerEventData eventData)
    {
        _gameController.StartGame();
        Runner.Despawn(Object);
    }
}
