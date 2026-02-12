using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerDebugger))]
public class PlayerDebuggerEditor : Editor
{
    private const double RepaintInterval = 0.1;
    private double _lastRepaintTime;

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
        DrawDefaultInspector();

        EditorGUILayout.Space(4);
        var debugger = (PlayerDebugger)target;
        Player player = debugger.PlayerRef;

        if (Application.isPlaying && player != null && player.Model != null)
        {
            var model = player.Model;
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
            if (player == null)
            {
                Debug.LogWarning("[PlayerDebugger] Player 참조가 없습니다. 인스펙터에서 반드시 할당하세요.");
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
