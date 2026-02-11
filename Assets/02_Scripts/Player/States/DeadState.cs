using UnityEngine;

/// <summary>
/// 사망 상태. 이동 차단, Dead 애니, 일정 시간 후 마을 포탈에서 부활 후 Idle로.
/// </summary>
public class DeadState : PlayerStateBase
{
    private float _deadTimer;
    private const float RespawnDelay = 2f;

    public DeadState(PlayerStateMachine machine, Player player) : base(machine, player) { }

    public override void Enter()
    {
        if (Player != null)
        {
            Player.CanMove = false;
            Player.SetCharacterControllerEnabled(false); // 몬스터 감지·공격 대상에서 제외
        }
        Player?.Animator?.Dead();
        _deadTimer = 0f;
    }

    public override void Update()
    {
        _deadTimer += Time.deltaTime;
        if (_deadTimer >= RespawnDelay && Player != null)
        {
            GameEvents.OnRespawnRequested?.Invoke(Player);
        }
    }

    public override void Exit()
    {
        if (Player != null)
        {
            Player.CanMove = true;
            Player.SetCharacterControllerEnabled(true); // 부활 후 다시 감지 가능
        }
    }
}
