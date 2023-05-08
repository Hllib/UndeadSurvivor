using Fusion;
using System;
using System.Collections;
using UnityEngine;

public class NetworkPlayerAnimator : NetworkBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private RuntimeAnimatorController[] _runtimeAnimatorControllers;
    [Networked(OnChanged = nameof(OnAnimatorIdChanged))] public int animatorId { get; set; }
    private NetworkPlayer _player;
    private PlayerStates _currentState;

    private void Awake()
    {
        _player = GetComponentInParent<NetworkPlayer>();
        _player.OnPlayerDead += OnPlayerDead;
    }

    public static void OnAnimatorIdChanged(Changed<NetworkPlayerAnimator> changed)
    {
        changed.Behaviour.StartCoroutine(changed.Behaviour.ChangeSkin());
    }

    IEnumerator ChangeSkin()
    {
        yield return new WaitForSeconds(0.3f);
        UpdateAnimator(animatorId);
    }

    public void UpdateAnimator(int id)
    {
        switch (id)
        {
            case 2: _animator.runtimeAnimatorController = _runtimeAnimatorControllers[1]; break;
            case 3: _animator.runtimeAnimatorController = _runtimeAnimatorControllers[2]; break;
            default: break;
        }
    }

    public override void Spawned()
    {
        int id = PlayerPrefs.GetInt("Skin");
        if (HasInputAuthority)
        {
            RPC_ChangeAnimator(id);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_ChangeAnimator(int id)
    {
        animatorId = id;
    }

    private void OnPlayerDead(object sender, EventArgs e)
    {
        RPC_HidePlayer();
    }

    [Rpc]
    private void RPC_HidePlayer()
    {
        _animator.enabled = false;
        _spriteRenderer.enabled = false;
    }

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
        if (_animator.enabled)
        {
            if (inputData.moveDirection != Vector2.zero)
            {
                CurrentState = PlayerStates.Walk;
                _animator.SetFloat("xMove", inputData.moveDirection.x);
                _animator.SetFloat("yMove", inputData.moveDirection.y);
                FlipSpriteH(inputData.moveDirection.x);
            }
            else
            {
                CurrentState = PlayerStates.Idle;
            }
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
