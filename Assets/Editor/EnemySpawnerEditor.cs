using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemySpawner))]
public class EnemySpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(4);
        var spawner = (EnemySpawner)target;

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("적 생성 (SpawnEnemy)"))
            spawner.SpawnEnemy();
        EditorGUI.EndDisabledGroup();

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("플레이 모드에서만 버튼이 동작합니다.", MessageType.Info);
    }
}
