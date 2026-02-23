using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Enemy 생성. 팀(3마리) 단위로 스폰. CombatController 주입. 탐지·어그로는 Enemy가 자체 처리.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] [Tooltip("생성할 Enemy 프리팹 (Enemy·EnemyAggro 필요)")]
    private GameObject _enemyPrefab;
    [SerializeField] [Tooltip("비면 Spawner 위치에 생성")]
    private Transform _spawnPoint;
    [SerializeField] [Tooltip("PlayScene이 주입. 전투 등록용")]
    private CombatController _combatController;
    [SerializeField] [Tooltip("팀당 적 수 (기본 3)")]
    private int _teamSize = 3;
    [SerializeField] [Tooltip("동료 스폰 반경")]
    private float _spawnRadius = 2f;

    /// <summary>PlayScene에서 CombatController·초기 스폰 등 호출.</summary>
    public void Initialize(CombatController combatController)
    {
        _combatController = combatController;
        SpawnTeam();
    }

    /// <summary>팀(3마리) 한 묶음 생성. EnemyTeam 생성 후 각 Enemy에 Team·Aggro 설정.</summary>
    public EnemyTeam SpawnTeam()
    {
        if (_enemyPrefab == null || _combatController == null) return null;

        var teamObj = new GameObject("EnemyTeam");
        teamObj.transform.SetParent(transform);
        var team = teamObj.AddComponent<EnemyTeam>();
        team.Initialize(_combatController);

        Vector3 basePos = _spawnPoint != null ? _spawnPoint.position : transform.position;

        for (int i = 0; i < _teamSize; i++)
        {
            var offset = i == 0 ? Vector3.zero : new Vector3(
                Mathf.Cos(i * Mathf.PI * 2f / _teamSize) * _spawnRadius, 0f,
                Mathf.Sin(i * Mathf.PI * 2f / _teamSize) * _spawnRadius);

            Vector3 pos = basePos + offset;
            if (NavMesh.SamplePosition(pos, out var hit, _spawnRadius * 2f, NavMesh.AllAreas))
                pos = hit.position;

            var go = Object.Instantiate(_enemyPrefab, pos, Quaternion.identity, teamObj.transform);
            var enemy = go.GetComponent<Enemy>();
            if (enemy != null)
            {
                team.AddMember(enemy);
                enemy.SetTeam(team);
            }
        }

        return team;
    }
}
