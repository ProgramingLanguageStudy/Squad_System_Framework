using System.Collections.Generic;
using UnityEngine;

/// <summary>인벤토리 디버그/치트용. Hierarchy의 Debuggers 등에 붙이고, 인스펙터에서 반드시 Inventory 참조를 할당하세요.</summary>
public class InventoryDebugger : MonoBehaviour
{
    [SerializeField] [Tooltip("반드시 인스펙터에서 할당하세요. 참조 없으면 버튼 동작하지 않음.")]
    private Inventory _inventory;

    [System.Serializable]
    public class AddItemEntry
    {
        public ItemData itemData;
        public int amount = 1;
    }

    [Header("아이템 추가 리스트 (각 항목별 '추가' 버튼으로 등록)")]
    [SerializeField] private List<AddItemEntry> _addItemEntries = new List<AddItemEntry>();

    public Inventory InventoryRef => _inventory;
    public IReadOnlyList<AddItemEntry> AddItemEntries => _addItemEntries;
}
