using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryDebugger))]
public class InventoryDebuggerEditor : Editor
{
    private SerializedProperty _inventoryProp;
    private SerializedProperty _addItemEntriesProp;

    private void OnEnable()
    {
        _inventoryProp = serializedObject.FindProperty("_inventory");
        _addItemEntriesProp = serializedObject.FindProperty("_addItemEntries");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_inventoryProp);

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("아이템 추가 리스트 (각 항목별 행에서 추가)", EditorStyles.boldLabel);

        int count = _addItemEntriesProp.arraySize;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("항목 수", GUILayout.Width(50));
        int newSize = EditorGUILayout.IntField(count, GUILayout.Width(40));
        if (newSize != count)
            _addItemEntriesProp.arraySize = Mathf.Max(0, newSize);
        if (GUILayout.Button("+", GUILayout.Width(24))) _addItemEntriesProp.arraySize++;
        if (GUILayout.Button("-", GUILayout.Width(24)) && count > 0) _addItemEntriesProp.arraySize--;
        EditorGUILayout.EndHorizontal();

        var debugger = (InventoryDebugger)target;
        Inventory inventory = debugger.InventoryRef;

        for (int i = 0; i < _addItemEntriesProp.arraySize; i++)
        {
            var el = _addItemEntriesProp.GetArrayElementAtIndex(i);
            var itemDataProp = el.FindPropertyRelative("itemData");
            var amountProp = el.FindPropertyRelative("amount");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(itemDataProp, GUIContent.none, GUILayout.MinWidth(120));
            EditorGUILayout.PropertyField(amountProp, GUIContent.none, GUILayout.Width(50));
            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            if (GUILayout.Button("추가", GUILayout.Width(50)))
            {
                if (inventory == null)
                {
                    Debug.LogWarning("[InventoryDebugger] Inventory 참조가 없습니다. 인스펙터에서 반드시 할당하세요.");
                }
                else
                {
                    var itemData = itemDataProp.objectReferenceValue as ItemData;
                    int amount = amountProp.intValue;
                    if (itemData == null)
                        Debug.LogWarning("[InventoryDebugger] 해당 항목의 ItemData가 할당되지 않았습니다.");
                    else
                        inventory.AddItem(itemData, amount);
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        serializedObject.ApplyModifiedProperties();

        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("플레이 모드에서만 버튼이 동작합니다.", MessageType.Info);
    }
}
