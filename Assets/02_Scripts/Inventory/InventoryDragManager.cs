using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InventoryDragManager : Singleton<InventoryDragManager>
{
    [SerializeField] private Image _dragIcon;
    private InventorySlot _startSlot;

    protected override void Awake()
    {
        base.Awake();
        if (_dragIcon != null)
        {
            _dragIcon.gameObject.SetActive(false);
            _dragIcon.raycastTarget = false; // 드래그 중: 이걸 켜두면 아래 슬롯 감지가 막힘
        }
    }

    public void StartDrag(InventorySlot slot)
    {
        _startSlot = slot;
        _dragIcon.sprite = slot.GetIcon(); // 슬롯에서 Getter 사용
        _dragIcon.gameObject.SetActive(true);

        // 드래그 시작 시 툴팁이 겹치지 않도록 숨깁니다.
        if (TooltipUI.Instance != null) TooltipUI.Instance.Hide();
    }

    public void OnDrag(Vector2 mousePos)
    {
        _dragIcon.transform.position = mousePos;
    }

    public void EndDrag()
    {
        _dragIcon.gameObject.SetActive(false);

        // 1. 마우스 아래에 있는 슬롯을 찾습니다.
        InventorySlot targetSlot = GetSlotUnderMouse();

        // 2. 자기 자신이 아니고, 유효한 슬롯이면 교환!
        if (targetSlot != null && targetSlot != _startSlot)
        {
            InventoryManager.Instance.SwapItems(_startSlot.Index, targetSlot.Index);
            Debug.Log($"{_startSlot.name}과 {targetSlot.name} 슬롯 교환");
        }

        _startSlot = null;
    }

    /// <summary>
    /// 마우스 위치에 있는 UI 요소 중 InventorySlot 컴포넌트를 가진 오브젝트를 찾습니다.
    /// </summary>
    private InventorySlot GetSlotUnderMouse()
    {
        // 2. New Input System 으로 마우스 현재 위치 가져오기
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition; // 3. 가져온 좌표 설정

        List<RaycastResult> results = new List<RaycastResult>();

        // EventSystem이 null인지 체크 (에디터 전환 시 등)
        if (EventSystem.current == null) return null;

        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            // 부모 중 InventorySlot이 있는지 확인 (이미지가 아닌 텍스트가 클릭될 수도 있으므로 부모까지 검사)
            InventorySlot slot = result.gameObject.GetComponentInParent<InventorySlot>();
            if (slot != null) return slot;
        }

        return null;
    }
}
