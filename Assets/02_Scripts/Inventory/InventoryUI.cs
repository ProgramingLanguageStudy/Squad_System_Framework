using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _slotContainer;
    private List<InventorySlot> _slots = new List<InventorySlot>();

    private void Start()
    {
        // 슬롯 미리 생성 (예: 30개)
        for (int i = 0; i < 30; i++)
        {
            var slot = Instantiate(_slotPrefab, _slotContainer).GetComponent<InventorySlot>();
            slot.ClearSlot();
            _slots.Add(slot);
        }

        // 인벤토리 매니저의 이벤트 구독
        InventoryManager.Instance.OnItemChanged += (item, count) => Refresh();

        // 시작할 때 꺼두기
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
        var items = InventoryManager.Instance.GetAllItems();
        for (int i = 0; i < _slots.Count; i++)
        {
            if (i < items.Count)
                _slots[i].UpdateSlot(items[i].Key, items[i].Value);
            else
                _slots[i].ClearSlot();
        }
    }
}