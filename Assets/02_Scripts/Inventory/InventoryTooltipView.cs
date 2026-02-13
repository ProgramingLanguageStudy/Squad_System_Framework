using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>인벤토리 오른쪽 상세 패널(아이템 설명 + USE 버튼). InventoryView가 보유. 슬롯 클릭 시 Show, USE 버튼으로 사용.</summary>
public class InventoryTooltipView : MonoBehaviour
{
    [SerializeField] private GameObject _tooltipPanel;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] [Tooltip("이 버튼을 누르면 현재 선택된 슬롯의 아이템 사용")]
    private Button _useButton;

    private int _selectedSlotIndex = -1;

    /// <summary>지금 패널에 표시 중인 슬롯 인덱스. 없으면 -1. View가 해당 슬롯 비었을 때 툴팁 닫는 데 사용.</summary>
    public int ShownSlotIndex => _selectedSlotIndex;

    /// <summary>USE 버튼 클릭 시 (선택된 슬롯 인덱스). Presenter가 구독해 TryUseItem 호출.</summary>
    public event Action<int> OnUseRequested;

    private void Awake()
    {
        if (_useButton != null)
            _useButton.onClick.AddListener(HandleUseButtonClicked);
    }

    private void OnDestroy()
    {
        if (_useButton != null)
            _useButton.onClick.RemoveListener(HandleUseButtonClicked);
    }

    /// <summary>초기 상태로 설정. InventoryView.Initialize()에서 호출. 패널 숨김.</summary>
    public void Initialize()
    {
        Hide();
    }

    private void HandleUseButtonClicked()
    {
        if (_selectedSlotIndex >= 0)
            OnUseRequested?.Invoke(_selectedSlotIndex);
    }

    /// <summary>해당 슬롯을 선택하고 오른쪽 패널에 표시. 슬롯 클릭(터치) 시 View가 호출.</summary>
    public void Show(ItemSlotModel slot)
    {
        if (_tooltipPanel == null) return;
        if (slot == null || slot.Item == null)
        {
            Hide();
            return;
        }

        _selectedSlotIndex = slot.Index;
        if (_icon != null)
        {
            _icon.sprite = slot.Item.Icon;
            _icon.enabled = slot.Item.Icon != null;
        }
        if (_nameText != null) _nameText.text = slot.Item.ItemName;
        if (_descText != null) _descText.text = slot.Item.Description ?? string.Empty;
        if (_useButton != null) _useButton.interactable = true;
        _tooltipPanel.SetActive(true);
    }

    /// <summary>패널 숨김. 인벤토리 닫을 때 또는 드래그 시작 시.</summary>
    public void Hide()
    {
        _selectedSlotIndex = -1;
        if (_icon != null)
        {
            _icon.sprite = null;
            _icon.enabled = false;
        }
        if (_tooltipPanel != null)
            _tooltipPanel.SetActive(false);
        if (_useButton != null)
            _useButton.interactable = false;
    }
}
