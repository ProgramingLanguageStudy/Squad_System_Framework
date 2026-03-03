using UnityEngine;

/// <summary>
/// 이동 의도 적용. 플레이어(Direction) / 동료(Target) 공통 계약.
/// SetAsPlayer / SetAsCompanion 시 Handler 교체.
/// </summary>
public interface IMovementHandler
{
    void SetDirectionIntent(Vector3 worldDirection);
    void SetTargetIntent(Transform target, float stopDistance);
    void ClearTargetIntent();
    /// <summary>Idle 전환 시 방향 intent 클리어. 특수 흐름에서 이전 입력으로 움직이는 것 방지.</summary>
    void ClearDirectionIntent();
    void Apply();
}
