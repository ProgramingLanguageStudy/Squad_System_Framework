using UnityEngine;

/// <summary>인벤토리 디버그/치트용. Hierarchy의 Debuggers 등에 붙이고, 인스펙터에서 Inventory 할당 (비면 플레이 시 Find 시도).</summary>
public class InventoryDebugger : MonoBehaviour
{
    [SerializeField] [Tooltip("비워두면 플레이 모드에서 FindFirstObjectByType으로 찾음")]
    private Inventory _inventory;

    [Header("테스트 아이템 추가용 (버튼에서 사용)")]
    [SerializeField] private ItemData _testItemData;
    [SerializeField] private int _testAmount = 1;

    public Inventory InventoryRef => _inventory;
    public ItemData TestItemData => _testItemData;
    public int TestAmount => _testAmount;
}
