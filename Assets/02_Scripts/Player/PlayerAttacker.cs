using UnityEngine;

/// <summary>
/// 공격 애니메이션 종료만 담당. 입력은 Player.RequestAttack()으로 처리.
/// 애니메이션 이벤트에서 EndAttack() 호출 시 RequestIdle()로 Idle 복귀.
/// </summary>
public class PlayerAttacker : MonoBehaviour
{
    private PlayerStateMachine _stateMachine;

    public void Initialize(PlayerStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    /// <summary>공격 애니메이션 끝날 때 호출. 애니메이션 이벤트에서 연결.</summary>
    public void EndAttack()
    {
        _stateMachine?.RequestIdle();
    }
}
