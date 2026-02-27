#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PortalDebugger))]
public class PortalDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 속성들(PortalController 필드 등) 출력
        DrawDefaultInspector();

        PortalDebugger debugger = (PortalDebugger)target;
        PortalController pc = debugger.GetController();

        if (pc == null)
        {
            EditorGUILayout.HelpBox("PortalController를 찾을 수 없습니다.", MessageType.Error);
            return;
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Global Control", EditorStyles.boldLabel);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("전체 해금", GUILayout.Height(30)))
            {
                debugger.UnlockAll();
            }
            if (GUILayout.Button("전체 잠금", GUILayout.Height(30)))
            {
                debugger.LockAll();
            }
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Individual Portal Control", EditorStyles.boldLabel);

        if (pc.PortalModels != null)
        {
            foreach (var model in pc.PortalModels)
            {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    // 상태에 따라 레이블 색상 변경
                    GUI.contentColor = model.IsUnlocked ? Color.green : Color.gray;
                    EditorGUILayout.LabelField($"{model.Data.displayName} ({(model.IsUnlocked ? "Unlocked" : "Locked")})");
                    GUI.contentColor = Color.white;

                    if (GUILayout.Button("Toggle", GUILayout.Width(70)))
                    {
                        debugger.TogglePortal(model);
                        EditorUtility.SetDirty(pc); // 데이터 변경 알림
                    }
                }
            }
        }

        // 데이터가 변경되었을 수 있으므로 리페인트
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif