using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _itemData; // 이 오브젝트가 어떤 아이템인지
    [SerializeField] private int _amount = 1;    // 몇 개나 줄 것인지

    public string GetInteractText()
    {
        // 상호작용 가이드에 "사과 획득하기[E]"라고 뜨게 합니다.
        return $"{_itemData.ItemName} 획득하기[E]";
    }

    public void Interact(Player player)
    {
        // 1. 인벤토리 매니저에 아이템 추가
        InventoryManager.Instance.AddItem(_itemData, _amount);

        // 2. 바닥에서 아이템 제거 (혹은 풀링 처리)
        Debug.Log($"{_itemData.ItemName}을(를) 주웠습니다.");
        Destroy(gameObject);

        // 💡 팁: 여기서 '줍는 소리'나 '이펙트'를 실행하면 더 좋습니다.
    }
}