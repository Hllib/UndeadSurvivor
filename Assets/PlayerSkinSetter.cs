using Fusion;
using System.Collections;
using UnityEngine;

public class PlayerSkinSetter : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnAnimatorIdChanged))] public int animatorId { get; set; }
    private NetworkPlayerAnimator _playerAnimator;

    public static void OnAnimatorIdChanged(Changed<PlayerSkinSetter> changed)
    {
        changed.Behaviour.StartCoroutine(changed.Behaviour.ChangeSkin());
    }

    private void Start()
    {
        _playerAnimator = GetComponent<NetworkPlayerAnimator>();
    }

    IEnumerator ChangeSkin()
    {
        yield return new WaitForSeconds(0.3f);
        _playerAnimator.UpdateAnimator(animatorId);
    }
}
