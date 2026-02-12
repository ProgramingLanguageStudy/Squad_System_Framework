using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryDebugger))]
public class InventoryDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(4);
        var debugger = (InventoryDebugger)target;
        Inventory inventory = debugger.InventoryRef;
        if (inventory == null && Application.isPlaying)
            inventory = Object.FindFirstObjectByType<Inventory>();

        EditorGUI.BeginDisabledGroup(!Application.isPlaying);
        if (GUILayout.Button("테스트 아이템 추가"))
        {
            if (inventory == null)
            {
                Debug.LogWarning("[InventoryDebugger] Inventory가 할당되지 않았고 씬에서도 찾을 수 없습니다. 인스펙터에서 할당하거나 씬에 Inventory가 있는지 확인하세요.");
                return;
            }
            ItemData itemData = debugger.TestItemData;
            if (itemData == null)
            {
                Debug.LogWarning("[InventoryDebugger] 추가할 아이템(Test Item Data)이 할당되지 않았습니다. 인스펙터에서 ItemData를 지정하세요.");
                return;
            }
            inventory.AddItem(itemData, debugger.TestAmount);
        }
        EditorGUI.EndDisabledGroup();

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("플레이 모드에서만 버튼이 동작합니다.", MessageType.Info);
    }
}
