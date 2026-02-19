using UnityEngine;

/// <summary>
/// 월드에 떨어진 아이템. IInteractable로 상호작용 시 "획득"만 알림(이벤트 발행). 인벤토리를 모름.
/// </summary>
public class ItemObject : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _itemData;
    [SerializeField] private int _amount = 1;

    public string GetInteractText() => _itemData.ItemName;

    public void Interact(IInteractReceiver receiver)
    {
        if (_itemData == null) return;

        GameEvents.OnItemPickedUp?.Invoke(_itemData, _amount);
        Destroy(gameObject);
    }
}