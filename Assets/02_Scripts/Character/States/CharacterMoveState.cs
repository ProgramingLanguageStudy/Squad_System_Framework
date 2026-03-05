using UnityEngine;

public class CharacterMoveState : CharacterStateBase
{
    public override bool CanMove => true;

    /// <summary>플레이어: 입력 없으면 완료. 동료: AIBrain.RequestIdle로 종료.</summary>
    public override bool IsComplete => Character != null && Character.IsPlayer && !Character.HasMoveInput;

    public CharacterMoveState(CharacterStateMachine machine, Character character) : base(machine, character) { }

    public override void Enter() { }

    public override void Update()
    {
        Character?.ApplyMovement();
    }

    public override void Exit() { }
}
