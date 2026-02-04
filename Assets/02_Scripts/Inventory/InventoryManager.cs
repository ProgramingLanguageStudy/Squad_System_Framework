using System;
using UnityEngine;

[System.Serializable]
public class ItemSlotData
{
    public ItemData Item;
    public int Count;

    public ItemSlotData(ItemData item, int count)
    {
        Item = item;
        Count = count;
    }

    public void Clear()
    {
        Item = null;
        Count = 0;
    }
}

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] private int _inventorySize = 30;
    private ItemSlotData[] _slots; // 핵심: 인덱스가 있는 배열

    public event Action OnItemChanged; // UI 갱신용 이벤트

    protected override void Awake()
    {
        base.Awake();
        // 인벤토리 공간 초기화
        _slots = new ItemSlotData[_inventorySize];
        for (int i = 0; i < _inventorySize; i++)
        {
            _slots[i] = new ItemSlotData(null, 0);
        }
    }

    // --- 아이템 조작 로직 ---

    public void AddItem(ItemData item, int amount = 1)
    {
        // 1. 기존에 같은 아이템이 있는 슬롯을 찾아 여유 공간에 채워넣기
        if (item.IsStackable)
        {
            foreach (var slot in _slots)
            {
                if (slot.Item == item && slot.Count < item.MaxStack)
                {
                    int canAdd = item.MaxStack - slot.Count;
                    int amountToAdd = Mathf.Min(amount, canAdd);

                    slot.Count += amountToAdd;
                    amount -= amountToAdd;

                    if (amount <= 0)
                    {
                        OnItemChanged?.Invoke();
                        GameEvents.OnQuestGoalProcessed?.Invoke(item.ItemId, GetTotalCount(item.ItemId));
                        return;
                    }
                }
            }
        }

        // 2. 남은 수량이 있다면 빈 슬롯을 찾아 새로 넣기
        // 만약 한 번에 주운 양이 MaxStack보다 많을 경우를 대비해 루프를 돕니다.
        while (amount > 0)
        {
            int emptySlotIndex = FindEmptySlotIndex();

            if (emptySlotIndex == -1)
            {
                Debug.LogWarning("인벤토리가 가득 차서 남은 아이템을 버리거나 무시합니다: " + amount);
                break;
            }

            int amountToPut = Mathf.Min(amount, item.MaxStack);
            _slots[emptySlotIndex].Item = item;
            _slots[emptySlotIndex].Count = amountToPut;

            amount -= amountToPut;
        }

        OnItemChanged?.Invoke();

        // [추가] 퀘스트 시스템을 위해 이벤트 발생! 
        // "이 아이템(ID)의 현재 총 수량은 이만큼이야"라고 알려줍니다.
        GameEvents.OnQuestGoalProcessed?.Invoke(item.ItemId, GetTotalCount(item.ItemId));
    }

    // 빈 슬롯 찾는 헬퍼 함수
    private int FindEmptySlotIndex()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].Item == null) return i;
        }
        return -1;
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= _inventorySize || indexB < 0 || indexB >= _inventorySize) return;

        ItemSlotData slotA = _slots[indexA];
        ItemSlotData slotB = _slots[indexB];

        // [개선] 같은 아이템이고 합칠 수 있는 경우
        if (slotA.Item != null && slotA.Item == slotB.Item && slotA.Item.IsStackable)
        {
            int maxStack = slotB.Item.MaxStack;
            int canAdd = maxStack - slotB.Count; // B칸에 더 들어갈 수 있는 여유 공간

            if (canAdd > 0)
            {
                // A에서 뺄 수 있는 양 (가진 것 vs 여유 공간 중 작은 값)
                int amountToAdd = Mathf.Min(slotA.Count, canAdd);

                slotB.Count += amountToAdd;
                slotA.Count -= amountToAdd;

                // A칸의 아이템을 다 옮겼다면 제거
                if (slotA.Count <= 0)
                {
                    slotA.Clear();
                }
            }
            else
            {
                // B칸이 이미 꽉 찼다면 그냥 자리를 바꿉니다 (Swap)
                PerformSwap(slotA, slotB);
            }
        }
        else
        {
            // 다른 아이템이면 그냥 Swap
            PerformSwap(slotA, slotB);
        }

        OnItemChanged?.Invoke();

        // [추가] 슬롯이 합쳐지거나 위치가 바뀌면 전체 수량은 안 변해도 
        // 퀘스트 시스템이 한 번 더 체크하게 해주는 것이 안전합니다. (선택사항)
        if (_slots[indexB].Item != null)
        {
            GameEvents.OnQuestGoalProcessed?.Invoke(_slots[indexB].Item.ItemId, GetTotalCount(_slots[indexB].Item.ItemId));
        }
    }

    // 중복 코드를 방지하기 위한 Swap 전용 함수
    private void PerformSwap(ItemSlotData a, ItemSlotData b)
    {
        ItemData tempItem = a.Item;
        int tempCount = a.Count;

        a.Item = b.Item;
        a.Count = b.Count;

        b.Item = tempItem;
        b.Count = tempCount;
    }

    public ItemSlotData[] GetSlots() => _slots;

    /// <summary>
    /// 특정 아이템을 지정 수량만큼 제거합니다. (퀘스트 제출 등)
    /// </summary>
    /// <returns>요청한 수량만큼 제거했으면 true, 부족하면 false</returns>
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
            if (slot.Count <= 0)
                slot.Clear();
        }

        OnItemChanged?.Invoke();
        GameEvents.OnQuestGoalProcessed?.Invoke(itemId, GetTotalCount(itemId));
        return true;
    }

    /// <summary>
    /// 인벤토리 전체를 뒤져서 특정 아이템의 총 개수를 반환합니다.
    /// </summary>
    public int GetTotalCount(string itemId)
    {
        int total = 0;
        foreach (var slot in _slots)
        {
            if (slot.Item != null && slot.Item.ItemId == itemId)
            {
                total += slot.Count;
            }
        }
        return total;
    }
}