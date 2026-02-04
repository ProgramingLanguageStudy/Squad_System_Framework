using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _slotPrefab;
    [SerializeField] private Transform _slotContainer;
    [SerializeField] private int _maxSlotCount = 30;

    private List<InventorySlot> _slots = new List<InventorySlot>();

    private void Start()
    {
        StartCoroutine(CreateSlotsOverFrames());
    }

    /// <summary>슬롯 생성을 여러 프레임에 나눠 첫 실행 시 렉 완화.</summary>
    private IEnumerator CreateSlotsOverFrames()
    {
        const int slotsPerFrame = 5;
        for (int i = 0; i < _maxSlotCount; i++)
        {
            var slotGo = Instantiate(_slotPrefab, _slotContainer);
            var slot = slotGo.GetComponent<InventorySlot>();
            slot.SetIndex(i);
            slot.ClearSlot();
            _slots.Add(slot);
            if ((i + 1) % slotsPerFrame == 0)
                yield return null;
        }
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnItemChanged += () => Refresh();
        gameObject.SetActive(false);
    }

    public void ToggleInventory()
    {
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);

        if (isActive)
            Refresh();
        else if (TooltipUI.Instance != null)
            TooltipUI.Instance.Hide();
        // 커서 표시/숨김은 PlayScene.Update()에서 통합 처리
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