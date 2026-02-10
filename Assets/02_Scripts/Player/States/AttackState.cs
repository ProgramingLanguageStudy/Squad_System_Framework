using UnityEngine;

/// <summary>
/// 공격 중 상태. 이동 불가. Enter에서 Attack 트리거. 종료는 애니메이션 이벤트에서 Machine.RequestIdle().
/// </summary>
public class AttackState : PlayerStateBase
{
    public AttackState(PlayerStateMachine machine, Player player) : base(machine, player) { }

    public override void Enter()
    {
        if (Player != null)
            Player.CanMove = false;

        Player?.Animator?.Attack();
        Player?.Attacker?.OnAttackStarted();
    }

    public override void Update()
    {
        // 공격 종료는 애니메이션 이벤트에서 RequestIdle() 호출로 처리
    }

    public override void Exit()
    {
        Machine.RequestIdle();
        Player?.Attacker?.OnAttackEnded();
    }
}
