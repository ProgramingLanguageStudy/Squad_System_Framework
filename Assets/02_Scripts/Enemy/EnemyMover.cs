using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy 이동 담당. NavMeshAgent 래핑. 속도·정지·목표 설정·도착 판정.
/// 상태는 Enemy.Mover를 통해 호출만 함.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMover : MonoBehaviour
{
    private NavMeshAgent _agent;

    public void Initialize(NavMeshAgent agent)
    {
        _agent = agent;
    }

    /// <summary>이동 속도 설정. Patrol/Chase 전환 시 호출.</summary>
    public void SetSpeed(float speed)
    {
        if (_agent != null)
            _agent.speed = speed;
    }

    /// <summary>이동 정지 (Attack, Dead, Patrol 쉬기 구간 등).</summary>
    public void Stop()
    {
        if (_agent != null)
            _agent.isStopped = true;
    }

    /// <summary>이동 재개. 목표는 그대로 두고 isStopped만 해제.</summary>
    public void Resume()
    {
        if (_agent != null)
            _agent.isStopped = false;
    }

    /// <summary>목표 지점 설정. Chase 등에서 사용.</summary>
    public void SetDestination(Vector3 worldPosition)
    {
        if (_agent != null && _agent.isOnNavMesh)
            _agent.SetDestination(worldPosition);
    }

    /// <summary>반경 내 NavMesh 위 랜덤 지점으로 목표 설정. Patrol용.</summary>
    /// <returns>유효한 목표를 설정했으면 true</returns>
    public bool SetRandomDestinationInRadius(Vector3 center, float radius)
    {
        if (_agent == null) return false;
        Vector2 circle = Random.insideUnitCircle * radius;
        Vector3 target = center + new Vector3(circle.x, 0f, circle.y);
        if (NavMesh.SamplePosition(target, out NavMeshHit hit, radius * 2f, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
            return true;
        }
        return false;
    }

    /// <summary>현재 목표까지 도착했는지. (pathPending 해소 후 remainingDistance 기준)</summary>
    public bool HasArrived(float threshold = 0.5f)
    {
        if (_agent == null) return true;
        return !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + threshold;
    }
}
