using UnityEngine;

/// <summary>
/// 공격 중 상태. 이동 불가. Enter에서 Attack 트리거.
/// 종료: 애니 이벤트 Animation_OnAttackEnded → RequestIdle(). 애니 미연결 시 안전 타이머로 복귀.
/// </summary>
public class AttackState : PlayerStateBase
{
    private float _timer;
    private const float FallbackDuration = 1.5f; // 애니 이벤트 없을 때 이 시간 후 Idle 복귀

    public AttackState(PlayerStateMachine machine, Player player) : base(machine, player) { }

    public override void Enter()
    {
        _timer = 0f;
        if (Player != null)
            Player.CanMove = false;

        Player?.Animator?.Attack();
        Player?.Attacker?.OnAttackStarted();
    }

    public override void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= FallbackDuration)
            Machine.RequestIdle();
    }

    public override void Exit()
    {
        Player?.Attacker?.EndAttackCleanup();
    }
}
