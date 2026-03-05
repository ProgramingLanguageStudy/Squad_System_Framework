using UnityEngine;

public abstract class CharacterStateBase
{
    protected readonly CharacterStateMachine Machine;
    protected readonly Character Character;

    public virtual bool CanMove => false;

    /// <summary>이 상태가 끝났는가? true면 StateMachine이 Idle로 전환.</summary>
    public virtual bool IsComplete => false;

    protected CharacterStateBase(CharacterStateMachine machine, Character character)
    {
        Machine = machine;
        Character = character;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
