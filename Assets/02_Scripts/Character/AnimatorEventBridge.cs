using System;
using UnityEngine;

/// <summary>
/// Animator와 같은 GameObject에 두어, 애니메이션 이벤트를 이벤트로 발행.
/// Unity 애니 이벤트는 Animator가 붙은 GameObject의 컴포넌트만 호출 가능하므로,
/// Animator가 자식에 있을 때 이 브릿지를 Animator와 같은 GameObject에 추가.
/// Character가 이 이벤트들을 구독해 필요한 컴포넌트(Attacker 등)와 연결.
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimatorEventBridge : MonoBehaviour
{
    public event Action OnBeginHitWindow;
    public event Action OnEndHitWindow;
    public event Action OnAttackEnded;

    public void Animation_BeginHitWindow() => OnBeginHitWindow?.Invoke();
    public void Animation_EndHitWindow() => OnEndHitWindow?.Invoke();
    public void Animation_OnAttackEnded() => OnAttackEnded?.Invoke();
}
