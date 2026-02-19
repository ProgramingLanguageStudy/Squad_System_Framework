using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerDebugger))]
public class PlayerDebuggerEditor : Editor
{
    private const double RepaintInterval = 0.1;
    private double _lastRepaintTime;
    private List<string> _lastValidationIssues = new List<string>();
    private bool _hasValidated;

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if (!Application.isPlaying || target == null) return;
        double now = EditorApplication.timeSinceStartup;
        if (now - _lastRepaintTime >= RepaintInterval)
        {
            _lastRepaintTime = now;
            Repaint();
        }
    }

    public override void OnInspectorGUI()
    {
        var debugger = (PlayerDebugger)target;
        var so = new SerializedObject(debugger);
        var playerProp = so.FindProperty("_playerController");

        EditorGUILayout.PropertyField(playerProp);

        if (playerProp.objectReferenceValue == null && GUILayout.Button("씬에서 PlayerController 찾기"))
        {
            var found = FindAnyObjectByType<PlayerController>();
            if (found != null)
            {
                playerProp.objectReferenceValue = found;
                so.ApplyModifiedProperties();
            }
            else
            {
                EditorGUILayout.HelpBox("씬에 PlayerController가 없습니다.", MessageType.Warning);
            }
        }

        EditorGUILayout.Space(4);

        if (GUILayout.Button("설정 검증 (Validate Setup)"))
        {
            _hasValidated = true;
            debugger.ValidateSetup(out _lastValidationIssues);
            Repaint();
        }

        if (_hasValidated)
        {
            EditorGUILayout.Space(2);
            if (_lastValidationIssues.Count > 0)
                EditorGUILayout.HelpBox(string.Join("\n", _lastValidationIssues), MessageType.Warning);
            else
                EditorGUILayout.HelpBox("검증 통과: 부품 구성 정상", MessageType.Info);
        }

        so.ApplyModifiedProperties();
        EditorGUILayout.Space(8);

        DrawDefaultInspector();
        EditorGUILayout.Space(4);

        var pc = debugger.PlayerRef;

        if (Application.isPlaying && pc != null && pc.Model != null)
        {
            var model = pc.Model;
            EditorGUILayout.LabelField("현재 스탯", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("체력", $"{model.CurrentHp} / {model.MaxHp}");
            EditorGUILayout.LabelField("사망", model.IsDead ? "예" : "아니오");
            EditorGUILayout.LabelField("이동 속도", model.MoveSpeed.ToString("F1"));
            EditorGUILayout.LabelField("공격력", model.AttackPower.ToString());
            EditorGUILayout.LabelField("공격 속도", model.AttackSpeed.ToString("F2"));
            EditorGUILayout.LabelField("방어력", model.Defense.ToString());
            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
        }

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("체력 풀회복"))
        {
            if (pc == null)
            {
                Debug.LogWarning("[PlayerDebugger] PlayerController 참조가 없습니다. 인스펙터에서 할당하세요.");
                return;
            }
            if (pc.Model != null)
            {
                int need = pc.Model.MaxHp - pc.Model.CurrentHp;
                if (need > 0)
                    pc.Model.Heal(need);
            }
        }
        EditorGUI.EndDisabledGroup();

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("플레이 모드에서만 스탯 표시·체력 회복이 동작합니다.", MessageType.Info);
    }
}
