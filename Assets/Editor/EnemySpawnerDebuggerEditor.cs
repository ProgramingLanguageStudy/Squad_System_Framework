using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawnerDebugger))]
public class EnemySpawnerDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(4);
        var debugger = (EnemySpawnerDebugger)target;
        EnemySpawner spawner = debugger.SpawnerRef;
        if (spawner == null && Application.isPlaying)
            spawner = Object.FindFirstObjectByType<EnemySpawner>();

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("적 생성 (SpawnEnemy)"))
        {
            if (spawner == null)
            {
                Debug.LogWarning("[EnemySpawnerDebugger] EnemySpawner가 할당되지 않았고 씬에서도 찾을 수 없습니다. 인스펙터에서 할당하거나 씬에 EnemySpawner가 있는지 확인하세요.");
                return;
            }
            spawner.SpawnEnemy();
        }
        EditorGUI.EndDisabledGroup();

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("플레이 모드에서만 버튼이 동작합니다.", MessageType.Info);
    }
}
