using UnityEngine;

public class CharacterIdleState : CharacterStateBase
{
    public CharacterIdleState(CharacterStateMachine machine, Character character) : base(machine, character) { }

    public override void Enter()
    {
        if (Character != null)
            Character.CanMove = true;
    }

    public override void Update() { }

    public override void Exit() { }
}
