using UnityEngine;

/// <summary>
/// 대기/자유 상태. 이동·공격 입력 허용.
/// </summary>
public class IdleState : PlayerStateBase
{
    public IdleState(PlayerStateMachine machine, Player player) : base(machine, player) { }

    public override void Enter()
    {
        if (Player != null)
            Player.CanMove = true;
    }

    public override void Update()
    {
        // 자유 상태에서는 할 일 없음. 이동 등은 다른 컴포넌트가 처리.
    }

    public override void Exit()
    {
        // 정리할 것 없음
    }
}
