using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>아이템 슬롯 한 칸의 UI. MVP의 슬롯 View. 스왑 시 이벤트만 발행하며 상위 View를 알지 않음.</summary>
public class ItemSlot : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerClickHandler
{
    private const float DoubleClickTime = 0.3f;

    [Header("----- UI 참조 -----")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _countText;

    [Header("----- 슬롯 -----")]
    [SerializeField] private int _index;

    private ItemSlotModel _model;
    private Image _dragIcon;
    private float _lastClickTime;

    public int Index => _index;

    /// <summary>이 슬롯이 표시하는 데이터. Presenter가 RefreshSlot 시 설정.</summary>
    public ItemSlotModel Model => _model;

    /// <summary>드롭으로 스왑 요청 시 (fromIndex, toIndex). InventoryView 등이 구독해 처리.</summary>
    public event Action<int, int> OnSwapRequested;
    /// <summary>더블클릭으로 사용 요청 시 (slotIndex). 소모품 등.</summary>
    public event Action<int> OnUseRequested;

    /// <summary>InventoryView에서 주입. 드래그 시 따라다니는 공용 아이콘.</summary>
    public void SetDragIcon(Image dragIcon)
    {
        _dragIcon = dragIcon;
        if (_dragIcon != null)
            _dragIcon.raycastTarget = false;
    }

    public void SetIndex(int index)
    {
        _index = index;
        gameObject.name = $"Slot_{index:00}";
    }

    public void ClearSlot()
    {
        _model = null;
        _icon.sprite = null;
        _icon.enabled = false;
        _countText.text = string.Empty;
    }

    /// <summary>자기 SlotModel을 설정하고 그 내용으로 표시 갱신.</summary>
    public void SetModel(ItemSlotModel model)
    {
        _model = model;
        if (_model == null || _model.Item == null || _model.Count <= 0)
        {
            ClearSlot();
            return;
        }
        _icon.sprite = _model.Item.Icon;
        _icon.enabled = true;
        _countText.text = _model.Count > 1 ? _model.Count.ToString() : string.Empty;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_model == null || _model.Item == null) return;
        if (Time.unscaledTime - _lastClickTime < DoubleClickTime)
        {
            _lastClickTime = 0f;
            OnUseRequested?.Invoke(Index);
        }
        else
            _lastClickTime = Time.unscaledTime;
    }

    #region 마우스 호버 이벤트 (툴팁)

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_model != null && _model.Item != null && TooltipUI.Instance != null)
            TooltipUI.Instance.Show(_model.Item.ItemName, _model.Item.Description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Hide();
    }

    #endregion

    #region 드래그 이벤트 (슬롯 교환)

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_model == null || _model.Item == null || _dragIcon == null) return;
        _dragIcon.sprite = _model.Item.Icon;
        _dragIcon.gameObject.SetActive(true);
        _dragIcon.transform.position = eventData.position;
        if (TooltipUI.Instance != null) TooltipUI.Instance.Hide();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon != null && _dragIcon.gameObject.activeSelf)
            _dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon != null)
            _dragIcon.gameObject.SetActive(false);

        ItemSlot target = GetSlotAtPosition(eventData.position);
        if (target != null && target != this)
            OnSwapRequested?.Invoke(Index, target.Index);
    }

    private static ItemSlot GetSlotAtPosition(Vector2 screenPos)
    {
        if (EventSystem.current == null) return null;
        var eventData = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var result in results)
        {
            var slot = result.gameObject.GetComponentInParent<ItemSlot>();
            if (slot != null) return slot;
        }
        return null;
    }

    #endregion
}
