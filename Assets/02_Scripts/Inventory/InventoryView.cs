using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리 UI 표시만 담당 (MVP의 View). Model을 알지 않으며, Presenter가 전달한 슬롯 데이터로 그립니다.
/// </summary>
public class InventoryView : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryUIPanel;
    [SerializeField] [Tooltip("인벤토리 열 때 맨 위로 보이게 할 ScrollRect. 비면 동작 안 함")]
    private ScrollRect _scrollRect;
    [SerializeField] private Image _dragIcon;
    [SerializeField] private GameObject _itemSlotPrefab;
    [SerializeField] private Transform _slotGroup;
    [SerializeField] private int _maxSlotCount = 30;

    private List<ItemSlot> _slots = new List<ItemSlot>();

    /// <summary>인벤토리 패널이 켜져 있는지. PlayScene 등에서 커서/이동 제어용.</summary>
    public bool IsPanelActive => _inventoryUIPanel != null && _inventoryUIPanel.activeSelf;

    /// <summary>드롭 끝났을 때 (fromIndex, 스크린 좌표). Presenter가 GetSlotIndexAtPosition으로 목표 찾아 스왑.</summary>
    public event Action<int, Vector2> OnDropEnded;
    /// <summary>슬롯 더블클릭(사용) 요청 시. Presenter가 구독해 Model.TryUseItem 호출.</summary>
    public event Action<int> OnUseRequested;

    /// <summary>토글 시 Presenter가 구독해 Refresh 호출 유도.</summary>
    public event Action OnRefreshRequested;

    /// <summary>슬롯 생성·패널 초기 상태. Presenter가 호출.</summary>
    public void Initialize()
    {
        CreateSlots();
        if (_inventoryUIPanel != null)
            _inventoryUIPanel.SetActive(false);
    }

    /// <summary>슬롯을 한 번에 생성. 코루틴으로 나누면 패널이 먼저 꺼질 때 중단돼 30개 미만이 될 수 있음.</summary>
    private void CreateSlots()
    {
        for (int i = 0; i < _maxSlotCount; i++)
        {
            var slotGo = Instantiate(_itemSlotPrefab, _slotGroup);
            var slot = slotGo.GetComponent<ItemSlot>();
            slot.SetIndex(i);
            slot.OnDropEnded += (from, pos) => OnDropEnded?.Invoke(from, pos);
            slot.OnUseRequested += (idx) => OnUseRequested?.Invoke(idx);
            slot.SetDragIcon(_dragIcon);
            slot.ClearSlot();
            _slots.Add(slot);
        }
    }

    /// <summary>Presenter에서 호출. 해당 슬롯에 Model을 넣고 갱신.</summary>
    public void RefreshSlot(ItemSlotModel slotModel)
    {
        if (slotModel == null || slotModel.Index < 0 || slotModel.Index >= _slots.Count) return;
        _slots[slotModel.Index].SetModel(slotModel);
    }

    /// <summary>스크린 좌표가 속한 슬롯 인덱스. 없으면 -1. 스크롤 반영됨.</summary>
    public int GetSlotIndexAtPosition(Vector2 screenPos)
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            var rt = _slots[i].transform as RectTransform;
            if (rt == null) continue;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPos, null, out var local))
            {
                if (rt.rect.Contains(local))
                    return i;
            }
        }
        return -1;
    }

    public void ToggleInventory()
    {
        GameObject target = _inventoryUIPanel != null ? _inventoryUIPanel : gameObject;
        bool isActive = !target.activeSelf;
        target.SetActive(isActive);
        if (isActive)
        {
            if (_scrollRect != null)
                _scrollRect.verticalNormalizedPosition = 1f;
            OnRefreshRequested?.Invoke();
            GameEvents.OnCursorShowRequested?.Invoke();
        }
        else
        {
            if (TooltipUI.Instance != null)
                TooltipUI.Instance.Hide();
            GameEvents.OnCursorHideRequested?.Invoke();
        }
    }
}
