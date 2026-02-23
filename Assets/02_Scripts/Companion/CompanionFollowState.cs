using UnityEngine;

/// <summary>
/// 플레이어 따라가기 상태. GameServices.Player를 follow 타겟으로 설정.
/// </summary>
public class CompanionFollowState : CompanionStateBase
{
    public CompanionFollowState(CompanionStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        Machine.Character?.ClearCombatTarget();
        var player = GameServices.Player?.GetPlayer();
        Machine.Character?.SetFollowTarget(player != null ? player.transform : null);
    }

    public override void Update()
    {
        var player = GameServices.Player?.GetPlayer();
        Machine.Character?.SetFollowTarget(player != null ? player.transform : null);
    }

    public override void Exit() { }
}
