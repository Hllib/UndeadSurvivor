using Fusion;
using UnityEngine;

public class NetworkPlayerAnimator : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private PlayerStates _currentState;

    public enum PlayerStates
    {
        Idle, 
        Walk
    }

    PlayerStates CurrentState
    {
        set
        {
            _currentState = value;
            switch (_currentState)
            {
                case PlayerStates.Idle:
                    _animator.Play("Idle");
                    break;
                case PlayerStates.Walk:
                    _animator.Play("Walk");
                    break;
            }
        }
    }

    [Rpc]
    public void RPC_ChooseAnimation(NetworkInputData inputData)
    {
        if (inputData.direction != Vector2.zero)
        {
            CurrentState = PlayerStates.Walk;
            _animator.SetFloat("xMove", inputData.direction.x);
            _animator.SetFloat("yMove", inputData.direction.y);
            FlipSpriteH(inputData.direction.x);
        }
        else
        {
            CurrentState = PlayerStates.Idle;
        }
    }

    private void FlipSpriteH(float horizontalMove)
    {
        if (horizontalMove > 0) 
        {
            _spriteRenderer.flipX = false;
        }
        if (horizontalMove < 0) 
        {
            _spriteRenderer.flipX = true;
        }
    }
}
