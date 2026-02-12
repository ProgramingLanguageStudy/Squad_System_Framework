using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerDebugger))]
public class PlayerDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(4);
        var debugger = (PlayerDebugger)target;
        Player player = debugger.PlayerRef;
        if (player == null && Application.isPlaying)
            player = Object.FindFirstObjectByType<Player>();

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("체력 풀회복"))
        {
            if (player == null)
            {
                Debug.LogWarning("[PlayerDebugger] Player가 할당되지 않았고 씬에서도 찾을 수 없습니다. 인스펙터에서 할당하거나 씬에 Player가 있는지 확인하세요.");
                return;
            }
            int need = player.Model.MaxHp - player.Model.CurrentHp;
            if (need > 0)
                player.Model.Heal(need);
        }
        EditorGUI.EndDisabledGroup();

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("플레이 모드에서만 버튼이 동작합니다.", MessageType.Info);
    }
}
