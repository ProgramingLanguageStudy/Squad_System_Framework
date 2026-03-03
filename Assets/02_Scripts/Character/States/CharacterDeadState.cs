using UnityEngine;

/// <summary>
/// 죽음 상태. 모든 죽음 시각/물리 처리는 CharacterDeathHandler에 위임.
/// 사운드·이펙트 추가 시 DeathHandler.Handle()만 수정.
/// </summary>
public class CharacterDeadState : CharacterStateBase
{
    private float _deadTimer;
    private const float RespawnDelay = 2f;

    public CharacterDeadState(CharacterStateMachine machine, Character character) : base(machine, character) { }

    public override void Enter()
    {
        if (Character?.DeathHandler == null)
        {
            Debug.LogError($"[CharacterDeadState] {Character?.gameObject.name}: DeathHandler 없음. Character에 RequireComponent 확인.");
            _deadTimer = 0f;
            return;
        }
        Character.DeathHandler.Handle();
        _deadTimer = 0f;
    }

    public override void Update()
    {
        _deadTimer += Time.deltaTime;
        if (_deadTimer >= RespawnDelay && Character != null)
        {
            GameEvents.OnCharacterRespawnRequested?.Invoke(Character);
        }
    }

    public override void Exit()
    {
        if (Character != null)
        {
            Character.SetCharacterControllerEnabled(true);
        }
    }
}
