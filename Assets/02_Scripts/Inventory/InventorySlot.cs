using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler // 드래그 인터페이스 추가
{
    [Header("----- UI 참조 -----")]
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _countText;

    [Header("----- 슬롯 -----")]
    [SerializeField] private int _index; // 이 슬롯이 몇 번째 슬롯인지 (Inspector에서 설정)

    // 현재 슬롯에 들어 있는 아이템
    private ItemData _currentItem;
    private int _currentCount;

    // 인벤토리 시스템( DragManager)에서 Index를 넘겨 받을 때 씁니다.
    public int Index => _index;

    // 드래그용 Sprite 반환용 Getter
    public Sprite GetIcon() => _currentItem != null ? _currentItem.Icon : null;

    public void SetIndex(int index)
    {
        _index = index;
        // 게임 오브젝트 이름을 슬롯 순서에 맞게 바꿔줍니다.
        gameObject.name = $"Slot_{index:00}";
    }

    /// <summary>
    /// 슬롯 비우기
    /// </summary>
    public void ClearSlot()
    {
        _currentItem = null;
        _currentCount = 0;

        _icon.sprite = null;
        _icon.enabled = false;
        _countText.text = string.Empty;
    }

    /// <summary>
    /// 슬롯에 아이템 데이터 표시
    /// </summary>
    public void UpdateSlot(ItemData item, int count)
    {
        if (item == null || count <= 0)
        {
            ClearSlot();
            return;
        }

        _currentItem = item;
        _currentCount = count;

        _icon.sprite = _currentItem.Icon;
        _icon.enabled = true;
        _countText.text = _currentCount > 1 ? _currentCount.ToString() : string.Empty;
    }

    #region 마우스 호버 이벤트 (툴팁)

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_currentItem != null && TooltipUI.Instance != null)
        {
            TooltipUI.Instance.Show(_currentItem.ItemName, _currentItem.Description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipUI.Instance != null)
        {
            TooltipUI.Instance.Hide();
        }
    }

    #endregion

    #region 드래그 이벤트 (슬롯 교환)

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 아이템이 있을 때만 드래그 시작
        if (_currentItem != null && InventoryDragManager.Instance != null)
        {
            InventoryDragManager.Instance.StartDrag(this);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (InventoryDragManager.Instance != null)
        {
            // 마우스 위치를 실시간 전달 (eventData.position 사용)
            InventoryDragManager.Instance.OnDrag(eventData.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (InventoryDragManager.Instance != null)
        {
            InventoryDragManager.Instance.EndDrag();
        }
    }

    #endregion
}
