using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartGameButton : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private GameController _gameController;

    public void OnPointerClick(PointerEventData eventData)
    {
        _gameController.StartGame();
        Runner.Despawn(Object);
    }
}
