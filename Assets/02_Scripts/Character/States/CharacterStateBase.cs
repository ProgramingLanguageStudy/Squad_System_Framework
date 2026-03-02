using UnityEngine;

public abstract class CharacterStateBase
{
    protected readonly CharacterStateMachine Machine;
    protected readonly Character Character;

    public virtual bool CanMove => false;

    protected CharacterStateBase(CharacterStateMachine machine, Character character)
    {
        Machine = machine;
        Character = character;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
