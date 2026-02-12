using UnityEngine;

/// <summary>EnemySpawner 디버그/치트용. Hierarchy의 Debuggers 등에 붙이고, 인스펙터에서 EnemySpawner 할당 (비면 플레이 시 Find 시도).</summary>
public class EnemySpawnerDebugger : MonoBehaviour
{
    [SerializeField] [Tooltip("비워두면 플레이 모드에서 FindFirstObjectByType으로 찾음")]
    private EnemySpawner _spawner;

    public EnemySpawner SpawnerRef => _spawner;
}
