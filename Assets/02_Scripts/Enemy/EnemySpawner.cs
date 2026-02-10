using UnityEngine;

/// <summary>
/// Enemy 프리팹 생성. PlayScene이 chase target(Player 등)을 주입하고, SpawnEnemy 시 해당 target을 각 Enemy에 주입.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] [Tooltip("생성할 Enemy 프리팹 (루트에 Enemy 컴포넌트)")]
    private GameObject _enemyPrefab;
    [SerializeField] [Tooltip("비면 Spawner 위치에 생성")]
    private Transform _spawnPoint;

    private Transform _chaseTarget;

    /// <summary>PlayScene 등이 추적 목표(Player.transform) 주입 시 호출. 임시로 적 한 마리 생성.</summary>
    public void Initialize(Transform chaseTarget)
    {
        _chaseTarget = chaseTarget;
        SpawnEnemy();
    }

    /// <summary>Enemy 한 마리 생성. _spawnPoint 또는 Spawner 위치에 생성 후 chase target 주입.</summary>
    public Enemy SpawnEnemy()
    {
        if (_enemyPrefab == null) return null;

        Vector3 position = _spawnPoint != null ? _spawnPoint.position : transform.position;
        GameObject go = Instantiate(_enemyPrefab, position, Quaternion.identity);
        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null && _chaseTarget != null)
            enemy.SetChaseTarget(_chaseTarget);

        return enemy;
    }
}
