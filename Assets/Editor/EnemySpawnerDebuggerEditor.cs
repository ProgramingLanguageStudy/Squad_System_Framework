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

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("적 생성 (SpawnEnemy)"))
        {
            if (spawner == null)
            {
                Debug.LogWarning("[EnemySpawnerDebugger] EnemySpawner 참조가 없습니다. 인스펙터에서 반드시 할당하세요.");
                return;
            }
            spawner.SpawnEnemy();
        }
        EditorGUI.EndDisabledGroup();

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("플레이 모드에서만 버튼이 동작합니다.", MessageType.Info);
    }
}
