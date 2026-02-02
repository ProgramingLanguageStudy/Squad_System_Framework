using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // [독립의 핵심] 아이템이 변할 때 울리는 벨 (누가 듣든 상관없음)
    // 파라미터: (변화된 아이템, 현재 총 개수)
    public event Action<ItemData, int> OnItemChanged;

    private Dictionary<ItemData, int> _items = new Dictionary<ItemData, int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 아이템 추가
    public void AddItem(ItemData item, int amount = 1)
    {
        if (item == null) return;

        if (_items.ContainsKey(item)) _items[item] += amount;
        else _items.Add(item, amount);

        // 벨을 울린다 (구독자가 있을 때만)
        OnItemChanged?.Invoke(item, _items[item]);
    }

    // 아이템 삭제 (보상 지급이나 제작 시 필요)
    public void RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null || !_items.ContainsKey(item)) return;

        _items[item] -= amount;
        if (_items[item] <= 0) _items.Remove(item);

        OnItemChanged?.Invoke(item, GetItemCount(item));
    }
    
    // 현재 개수 확인
    public int GetItemCount(ItemData item)
    {
        return _items.ContainsKey(item) ? _items[item] : 0;
    }

    public List<KeyValuePair<ItemData, int>> GetAllItems()
    {
        // Dictionary를 리스트 형태로 변환해서 UI에 넘겨줍니다.
        return new List<KeyValuePair<ItemData, int>>(_items);
    }
}