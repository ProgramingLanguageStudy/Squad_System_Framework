using UnityEngine;

/// <summary>
/// 플레이어 상태 베이스. Enter / Update / Exit 세 가지로 생명주기.
/// </summary>
public abstract class PlayerStateBase
{
    protected readonly PlayerStateMachine Machine;
    protected readonly Player Player;

    protected PlayerStateBase(PlayerStateMachine machine, Player player)
    {
        Machine = machine;
        Player = player;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
