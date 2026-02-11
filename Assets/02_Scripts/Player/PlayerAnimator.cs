using UnityEngine;

/// <summary>
/// 플레이어 Animator 래퍼. 파라미터는 AnimatorParams 사용.
/// 이동 속도·공격 등 애니 실행은 이 클래스의 메서드로만 수행 (의존성은 Initialize로 주입).
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private PlayerMover _mover;

    private const float DampTime = 0.1f;

    public void Initialize(Animator animator, PlayerMover mover)
    {
        _animator = animator;
        _mover = mover;
    }

    private void Update()
    {
        if (_mover != null)
            Move(_mover.GetCurrentSpeed());
    }

    /// <summary>이동 속도 파라미터 설정. (블렌드 트리 등)</summary>
    public void Move(float moveSpeed)
    {
        if (_animator != null)
            _animator.SetFloat(AnimatorParams.MoveSpeed, moveSpeed, DampTime, Time.deltaTime);
    }

    /// <summary>공격 트리거 1회 발동.</summary>
    public void Attack()
    {
        if (_animator != null)
            _animator.SetTrigger(AnimatorParams.Attack);
    }

    /// <summary>사망 트리거 1회 발동. Dead 상태 진입 시 호출.</summary>
    public void Dead()
    {
        if (_animator != null)
            _animator.SetTrigger(AnimatorParams.Dead);
    }

    /// <summary>애니메이터를 Idle로 강제 전환. 부활 시 누운 상태 해제용. 상태 이름이 다르면 이 메서드에서 변경.</summary>
    public void ResetToIdle()
    {
        if (_animator != null)
            _animator.Play("Idle", 0, 0f);
    }
}
