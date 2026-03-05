using UnityEngine;

public class CharacterAttackState : CharacterStateBase
{
    public CharacterAttackState(CharacterStateMachine machine, Character character) : base(machine, character) { }

    public override bool IsComplete => Character?.Attacker?.IsAttackEnded == true;

    public override void Enter()
    {
        Character?.StopMovement();
        Character?.Attacker?.Begin();
    }

    public override void Update() { }

    public override void Exit()
    {
        Character?.Attacker?.End();
        Character?.Animator?.ResetToIdle();
    }
}
