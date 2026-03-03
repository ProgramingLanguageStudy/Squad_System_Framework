using UnityEngine;

/// <summary>플레이어용. 방향 기반 이동. CharacterMover 사용.</summary>
public class DirectionMovementHandler : IMovementHandler
{
    private readonly CharacterMover _mover;
    private Vector3 _directionIntent;

    public DirectionMovementHandler(CharacterMover mover)
    {
        _mover = mover;
    }

    public void SetDirectionIntent(Vector3 worldDirection)
    {
        _directionIntent = worldDirection;
    }

    public void SetTargetIntent(Transform target, float stopDistance)
    {
        // 플레이어는 Target 의도 사용 안 함
    }

    public void ClearTargetIntent() { }

    public void ClearDirectionIntent()
    {
        _directionIntent = Vector3.zero;
    }

    public void Apply()
    {
        _mover?.Move(_directionIntent);
    }
}
