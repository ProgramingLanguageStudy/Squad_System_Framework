using UnityEngine;

/// <summary>
/// 플레이어 따라가기 상태. PlaySceneServices.Player를 follow 타겟으로 설정.
/// </summary>
public class CompanionFollowState : CompanionStateBase
{
    public CompanionFollowState(CompanionStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        Machine.Character?.ClearCombatTarget();
        var player = PlaySceneServices.Player?.GetPlayer();
        Machine.Character?.SetFollowTarget(player != null ? player.transform : null);
    }

    public override void Update()
    {
        var player = PlaySceneServices.Player?.GetPlayer();
        Machine.Character?.SetFollowTarget(player != null ? player.transform : null);
    }

    public override void Exit() { }
}
