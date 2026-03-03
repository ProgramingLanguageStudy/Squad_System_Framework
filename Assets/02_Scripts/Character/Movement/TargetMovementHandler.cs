using UnityEngine;

/// <summary>동료용. 목표 기반 이동. CharacterFollowMover 사용.</summary>
public class TargetMovementHandler : IMovementHandler
{
    private readonly CharacterFollowMover _followMover;
    private readonly CharacterModel _model;
    private Transform _target;
    private float _stopDistance;

    public TargetMovementHandler(CharacterFollowMover followMover, CharacterModel model)
    {
        _followMover = followMover;
        _model = model;
    }

    public void SetDirectionIntent(Vector3 worldDirection)
    {
        // 동료는 Direction 의도 사용 안 함
    }

    public void SetTargetIntent(Transform target, float stopDistance)
    {
        _target = target;
        _stopDistance = stopDistance;
    }

    public void ClearTargetIntent()
    {
        _target = null;
    }

    public void ClearDirectionIntent()
    {
        // 동료는 Direction 의도 사용 안 함
    }

    public void Apply()
    {
        if (_target == null) return;
        _followMover?.MoveToTarget(_target.position, _stopDistance);
    }
}
