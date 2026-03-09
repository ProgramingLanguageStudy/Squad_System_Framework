using UnityEngine;

/// <summary>
/// 월드에 떨어진 아이템. IInteractable로 상호작용 시 "획득"만 알림(이벤트 발행). 인벤토리를 모름.
/// 인스펙터 설정 또는 Initialize로 런타임 설정 가능.
/// </summary>
public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private int _amount = 1;

    /// <summary>드롭 스폰 시 런타임 설정. 인스펙터 값 무시.</summary>
    public void Initialize(ItemData itemData, int amount)
    {
        _itemData = itemData;
        _amount = amount;
    }

    public string GetInteractText() => _itemData != null ? _itemData.ItemName : "";

    public void Interact(IInteractReceiver receiver)
    {
        if (_itemData == null) return;

        GameEvents.OnItemPickedUp?.Invoke(_itemData, _amount);

        var poolable = GetComponent<Poolable>();
        if (poolable != null)
            poolable.ReturnToPool();
        else
            Destroy(gameObject);
    }
}