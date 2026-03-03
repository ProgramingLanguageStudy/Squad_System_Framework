using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 죽음 처리. 이동 정지, 애니메이션, 컨트롤러 비활성화 등.
/// Dead 상태 Enter에서 호출. Character.Initialize에서 Inject 호출.
/// </summary>
[RequireComponent(typeof(Character))]
public class CharacterDeathHandler : MonoBehaviour
{
    [SerializeField] [Tooltip("인스펙터에서 할당 (없으면 Inject로 주입)")]
    private Character _character;
    [SerializeField] [Tooltip("인스펙터에서 할당 (없으면 Inject로 주입)")]
    private NavMeshAgent _navMeshAgent;

    /// <summary>Character.Initialize에서 호출. 인스펙터 미연결 시 주입.</summary>
    public void Initialize(Character character, NavMeshAgent navMeshAgent)
    {
        if (_character == null) _character = character;
        if (_navMeshAgent == null) _navMeshAgent = navMeshAgent;
    }

    private void Awake()
    {
        if (_character == null) _character = GetComponent<Character>();
        if (_navMeshAgent == null) _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary>죽음 시 처리. DeadState.Enter에서 호출.</summary>
    public void Handle()
    {
        if (_character == null) return;

        _character.StopMovement();
        _character.PlayDeadAnimation();
        _character.SetCharacterControllerEnabled(false);

        if (_navMeshAgent != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.velocity = Vector3.zero;
            _navMeshAgent.ResetPath();
        }
    }
}
