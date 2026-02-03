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
            _dragIcon.raycastTarget = false; // �ſ� �߿�: �̰� ���� ������ ���� ���� ����
        }
    }

    public void StartDrag(InventorySlot slot)
    {
        _startSlot = slot;
        _dragIcon.sprite = slot.GetIcon(); // �Ʊ� ���� Getter ���
        _dragIcon.gameObject.SetActive(true);

        // �巡�� ���� �� ������ ���صǹǷ� ����ϴ�.
        if (TooltipUI.Instance != null) TooltipUI.Instance.Hide();
    }

    public void OnDrag(Vector2 mousePos)
    {
        _dragIcon.transform.position = mousePos;
    }

    public void EndDrag()
    {
        _dragIcon.gameObject.SetActive(false);

        // 1. ���콺 �Ʒ��� �ִ� ������ ã���ϴ�.
        InventorySlot targetSlot = GetSlotUnderMouse();

        // 2. �ڱ� �ڽ��� �ƴϰ�, ��ȿ�� �����̶�� ��ġ ��ȯ!
        if (targetSlot != null && targetSlot != _startSlot)
        {
            InventoryManager.Instance.SwapItems(_startSlot.Index, targetSlot.Index);
            Debug.Log($"{_startSlot.name}���� {targetSlot.name}���� ��ü �õ�");
        }

        _startSlot = null;
    }

    /// <summary>
    /// ���콺 ��ġ�� UI ��ҵ� �� InventorySlot ������Ʈ�� ���� ������Ʈ�� ã���ϴ�.
    /// </summary>
    private InventorySlot GetSlotUnderMouse()
    {
        // 2. New Input System ������� ���콺 ���� ��ġ ��������
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition; // 3. �о�� ��ǥ ����

        List<RaycastResult> results = new List<RaycastResult>();

        // EventSystem�� null���� üũ (�� ��ȯ ���� ���)
        if (EventSystem.current == null) return null;

        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            // �θ� �� InventorySlot�� �ִ��� Ȯ�� (�������̳� �ؽ�Ʈ�� Ŭ���ص� ������ ã�ƾ� �ϴϱ��)
            InventorySlot slot = result.gameObject.GetComponentInParent<InventorySlot>();
            if (slot != null) return slot;
        }

        return null;
    }
}