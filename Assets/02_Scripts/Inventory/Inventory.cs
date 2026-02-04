using System;
using UnityEngine;

/// <summary>
/// 인벤토리 상태·로직. 런타임 데이터(슬롯 배열)를 관리합니다.
/// OnSlotChanged: UI 갱신용(변경된 슬롯 하나). OnItemChangedWithId: 퀘스트 등용(itemId, 총 수량).
/// </summary>
public class Inventory : MonoBehaviour
{
    [SerializeField] private int _inventorySize = 30;
    private ItemSlotModel[] _slots;

    /// <summary>슬롯 하나 변경 시. UI는 해당 슬롯만 갱신하면 됨.</summary>
    public event Action<ItemSlotModel> OnSlotChanged;
    /// <summary>아이템별 총 수량 변경 시 (itemId, 새 총 수량). 퀘스트 등에서 사용.</summary>
    public event Action<string, int> OnItemChangedWithId;

    private void Awake()
    {
        _slots = new ItemSlotModel[_inventorySize];
        for (int i = 0; i < _inventorySize; i++)
            _slots[i] = new ItemSlotModel(null, 0, i);
    }

    public void AddItem(ItemData itemData, int amount = 1)
    {
        if (itemData == null) return;
        if (itemData.IsStackable)
        {
            foreach (var slot in _slots)
            {
                if (slot.Item != null && slot.Item.Data == itemData && slot.Count < itemData.MaxStack)
                {
                    int canAdd = itemData.MaxStack - slot.Count;
                    int amountToAdd = Mathf.Min(amount, canAdd);
                    slot.Count += amountToAdd;
                    amount -= amountToAdd;
                    OnSlotChanged?.Invoke(slot);
                    if (amount <= 0)
                    {
                        NotifyItemChangedWithId(itemData.ItemId);
                        return;
                    }
                }
            }
        }

        while (amount > 0)
        {
            int emptySlotIndex = FindEmptySlotIndex();
            if (emptySlotIndex == -1)
            {
                Debug.LogWarning("인벤토리가 가득 차서 남은 아이템을 버리거나 무시합니다: " + amount);
                break;
            }
            int amountToPut = Mathf.Min(amount, itemData.MaxStack);
            _slots[emptySlotIndex].Item = new ItemModel(itemData);
            _slots[emptySlotIndex].Count = amountToPut;
            amount -= amountToPut;
            OnSlotChanged?.Invoke(_slots[emptySlotIndex]);
        }
        NotifyItemChangedWithId(itemData.ItemId);
    }

    private int FindEmptySlotIndex()
    {
        for (int i = 0; i < _slots.Length; i++)
            if (_slots[i].Item == null) return i;
        return -1;
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= _inventorySize || indexB < 0 || indexB >= _inventorySize) return;

        ItemSlotModel slotA = _slots[indexA];
        ItemSlotModel slotB = _slots[indexB];

        if (slotA.Item != null && slotB.Item != null && slotA.Item.Data == slotB.Item.Data && slotA.Item.IsStackable)
        {
            int maxStack = slotB.Item.MaxStack;
            int canAdd = maxStack - slotB.Count;
            if (canAdd > 0)
            {
                int amountToAdd = Mathf.Min(slotA.Count, canAdd);
                slotB.Count += amountToAdd;
                slotA.Count -= amountToAdd;
                if (slotA.Count <= 0) slotA.Clear();
            }
            else
                PerformSwap(slotA, slotB);
        }
        else
            PerformSwap(slotA, slotB);

        OnSlotChanged?.Invoke(_slots[indexA]);
        OnSlotChanged?.Invoke(_slots[indexB]);
        if (_slots[indexA].Item != null)
            OnItemChangedWithId?.Invoke(_slots[indexA].Item.ItemId, GetTotalCount(_slots[indexA].Item.ItemId));
        if (_slots[indexB].Item != null && _slots[indexB].Item != _slots[indexA].Item)
            OnItemChangedWithId?.Invoke(_slots[indexB].Item.ItemId, GetTotalCount(_slots[indexB].Item.ItemId));
    }

    private static void PerformSwap(ItemSlotModel a, ItemSlotModel b)
    {
        var tempItem = a.Item;
        var tempCount = a.Count;
        a.Item = b.Item;
        a.Count = b.Count;
        b.Item = tempItem;
        b.Count = tempCount;
    }

    public ItemSlotModel[] GetSlots() => _slots;

    public bool RemoveItem(string itemId, int amount)
    {
        if (GetTotalCount(itemId) < amount) return false;

        int remaining = amount;
        foreach (var slot in _slots)
        {
            if (remaining <= 0) break;
            if (slot.Item == null || slot.Item.ItemId != itemId) continue;
            int toRemove = Mathf.Min(slot.Count, remaining);
            slot.Count -= toRemove;
            remaining -= toRemove;
            if (slot.Count <= 0) slot.Clear();
            OnSlotChanged?.Invoke(slot);
        }
        OnItemChangedWithId?.Invoke(itemId, GetTotalCount(itemId));
        return true;
    }

    public int GetTotalCount(string itemId)
    {
        int total = 0;
        foreach (var slot in _slots)
        {
            if (slot.Item != null && slot.Item.ItemId == itemId)
                total += slot.Count;
        }
        return total;
    }

    private void NotifyItemChangedWithId(string itemId)
    {
        OnItemChangedWithId?.Invoke(itemId, GetTotalCount(itemId));
    }
}
