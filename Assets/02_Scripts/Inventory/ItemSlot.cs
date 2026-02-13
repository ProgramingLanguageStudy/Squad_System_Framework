using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>아이템 슬롯 한 칸의 UI. MVP의 슬롯 View. 호버/선택은 스프라이트 교체, 클릭 시 상세 패널 표시. 사용은 USE 버튼만.</summary>
public class ItemSlot : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler,
    IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("----- UI 참조 -----")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] [Tooltip("테두리 바꿔끼우는 대상. 비면 이 오브젝트의 Image 사용")]
    private Image _slotImage;

    [Header("----- 슬롯 스프라이트 (프레임 없음/호버/선택) -----")]
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _hoverSprite;
    [SerializeField] private Sprite _selectedSprite;

    [Header("----- 슬롯 -----")]
    [SerializeField] private int _index;

    private ItemSlotModel _model;
    private Image _dragIcon;
    private bool _isSelected;
    private bool _isHovered;

    public int Index => _index;

    /// <summary>이 슬롯이 표시하는 데이터. Presenter가 RefreshSlot 시 설정.</summary>
    public ItemSlotModel Model => _model;

    /// <summary>드롭 끝났을 때 (fromIndex, 스크린 좌표). View가 위치로 목표 슬롯을 찾아 스왑 처리.</summary>
    public event Action<int, Vector2> OnDropEnded;
    /// <summary>한 번 클릭으로 슬롯 선택 시 (선택된 슬롯 모델). View가 구독해 상세 패널 표시.</summary>
    public event Action<ItemSlotModel> OnSlotSelected;
    /// <summary>드래그 시작 시. View가 구독해 상세 패널 숨김.</summary>
    public event Action OnDragStarted;

    private Image SlotImage => _slotImage != null ? _slotImage : GetComponent<Image>();

    /// <summary>선택 상태 설정. View가 슬롯 선택/패널 닫을 때 호출.</summary>
    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        RefreshSlotSprite();
    }

    private void RefreshSlotSprite()
    {
        var img = SlotImage;
        if (img == null) return;
        if (_isSelected && _selectedSprite != null)
            img.sprite = _selectedSprite;
        else if (_isHovered && _hoverSprite != null)
            img.sprite = _hoverSprite;
        else if (_normalSprite != null)
            img.sprite = _normalSprite;
    }

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
        _isSelected = false;
        _isHovered = false;
        RefreshSlotSprite();
        SetSlotRaycast(false);
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
        SetSlotRaycast(true);
    }

    /// <summary>빈 슬롯이면 레이캐스트 끄고(스크롤 통과), 아이템 있으면 슬롯 이미지에서만 받음. 아이콘은 raycast 끔.</summary>
    private void SetSlotRaycast(bool hasItem)
    {
        _icon.raycastTarget = false;
        var slotBg = SlotImage;
        if (slotBg != null)
            slotBg.raycastTarget = hasItem;
    }

    /// <summary>한 번 클릭 → 패널 표시. 사용은 상세 패널 USE 버튼만.</summary>
    private void HandleSlotClick()
    {
        if (_model == null || _model.Item == null) return;
        OnSlotSelected?.Invoke(_model);
    }

    public void OnPointerDown(PointerEventData eventData) => HandleSlotClick();

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("마우스 포인터 들어옴");
        if (_model == null || _model.Item == null) return;
        _isHovered = true;
        RefreshSlotSprite();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        RefreshSlotSprite();
    }

    #region 드래그 이벤트 (슬롯 교환)

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_model == null || _model.Item == null || _dragIcon == null) return;
        _dragIcon.sprite = _model.Item.Icon;
        _dragIcon.gameObject.SetActive(true);
        _dragIcon.transform.position = eventData.position;
        OnDragStarted?.Invoke();
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

        OnDropEnded?.Invoke(Index, eventData.position);
    }

    #endregion
}
