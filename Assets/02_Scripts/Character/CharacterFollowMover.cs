using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AI 동료 이동. NavMeshAgent로 대상(플레이어 등)을 따라감.
/// followDistance 안이면 정지, stopDistance 밖이면 이동. 멀어지면 catchUpSpeed로 속도 증가.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class CharacterFollowMover : MonoBehaviour
{
    private NavMeshAgent _agent;
    private CharacterModel _model;
    private Transform _followTarget;

    public void Initialize(NavMeshAgent agent, CharacterModel model)
    {
        _agent = agent;
        _model = model;
        if (_agent != null)
            _agent.updateRotation = true;
    }

    /// <summary>따라갈 목표(플레이어 등) 설정. null이면 정지.</summary>
    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
    }

    public float GetCurrentSpeed() => _agent != null && !_agent.isStopped ? _agent.velocity.magnitude : 0f;

    private void Update()
    {
        if (_agent == null || !_agent.isOnNavMesh || _model == null || _model.IsDead)
        {
            if (_agent != null) _agent.isStopped = true;
            return;
        }

        if (_followTarget == null)
        {
            _agent.isStopped = true;
            return;
        }

        Vector3 targetPos = _followTarget.position;
        float distSq = (targetPos - transform.position).sqrMagnitude;
        float stopSq = _model.StopDistance * _model.StopDistance;
        float followSq = _model.FollowDistance * _model.FollowDistance;

        if (distSq <= stopSq)
        {
            _agent.isStopped = true;
            _agent.speed = 0f;
            return;
        }

        _agent.isStopped = false;
        float baseSpeed = _model.MoveSpeed;
        _agent.speed = distSq > followSq ? baseSpeed * _model.CatchUpSpeed : baseSpeed;
        _agent.SetDestination(targetPos);
    }
}
