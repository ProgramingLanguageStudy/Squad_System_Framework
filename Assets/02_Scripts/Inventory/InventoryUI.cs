using System.Collections.Generic;
using UnityEngine;
using System.Linq; // ToList() 사용을 위해 필요

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _slotContainer;
    [SerializeField] private int _maxSlotCount = 30; // 상수로 관리하면 좋습니다.

    private List<InventorySlot> _slots = new List<InventorySlot>();

    private void Start()
    {
        // 1. 슬롯 미리 생성
        for (int i = 0; i < _maxSlotCount; i++)
        {
            var slotGo = Instantiate(_slotPrefab, _slotContainer);
            var slot = slotGo.GetComponent<InventorySlot>();

            // [추가] 생성되는 순서대로 슬롯에 인덱스(0, 1, 2...)를 부여합니다.
            slot.SetIndex(i);

            slot.ClearSlot();
            _slots.Add(slot);
        }

        // 2. 인벤토리 매니저 이벤트 구독
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnItemChanged += () => Refresh();
        }

        gameObject.SetActive(false);
    }

    public void ToggleInventory()
    {
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);

        if (isActive)
        {
            Refresh();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Refresh()
    {
        var slotsData = InventoryManager.Instance.GetSlots();

        // 내 UI 슬롯 개수와 데이터 배열 개수를 맞춰서 업데이트
        for (int i = 0; i < _slots.Count; i++)
        {
            if (i < slotsData.Length)
            {
                // 데이터의 Item과 Count를 슬롯 UI에 전달
                _slots[i].UpdateSlot(slotsData[i].Item, slotsData[i].Count);
            }
            else
            {
                _slots[i].ClearSlot();
            }
        }
    }
}