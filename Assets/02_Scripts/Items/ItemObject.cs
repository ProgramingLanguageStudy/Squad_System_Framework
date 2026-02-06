using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private int _amount = 1;
    [SerializeField] private Inventory _inventory;

    public string GetInteractText() => $"{_itemData.ItemName} 획득하기[E]";

    public void Interact(Player player)
    {
        var inv = _inventory != null ? _inventory : FindFirstObjectByType<Inventory>();
        if (inv != null)
            inv.AddItem(_itemData, _amount);

        // 2. 바닥에서 아이템 제거 (혹은 풀링 처리)
        Debug.Log($"{_itemData.ItemName}을(를) 주웠습니다.");
        Destroy(gameObject);

        // 💡 팁: 여기서 '줍는 소리'나 '이펙트'를 실행하면 더 좋습니다.
    }
}