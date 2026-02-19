using UnityEngine;

/// <summary>
/// Inventory(Model)와 InventoryView(View)를 연결. Model·View는 인스펙터에서 할당.
/// PlayScene이 SetPlayerController로 주입하고, 조종 캐릭터 변경 시 RefreshItemUser 호출.
/// </summary>
public class InventoryPresenter : MonoBehaviour
{
    [SerializeField] private Inventory _model;
    [SerializeField] private InventoryView _view;

    private PlayerController _playerController;

    /// <summary>PlayScene에서 주입. 조종 캐릭터 변경 시 RefreshItemUser 호출 필요.</summary>
    public void SetPlayerController(PlayerController controller)
    {
        _playerController = controller;
    }

    /// <summary>소모품 효과 적용 대상을 현재 조종 캐릭터로 갱신. 스쿼드 교체 후 호출.</summary>
    public void RefreshItemUser()
    {
        if (_model != null && _playerController != null)
            _model.SetItemUser(_playerController.Model);
    }

    private void Awake()
    {
        if (_model == null)
            Debug.LogWarning($"[InventoryPresenter] {gameObject.name}: Model(Inventory)이 할당되지 않았습니다. 인스펙터에서 할당해 주세요.");
        if (_view == null)
            Debug.LogWarning($"[InventoryPresenter] {gameObject.name}: View(InventoryView)가 할당되지 않았습니다. 인스펙터에서 할당해 주세요.");
        _model?.Initialize();
        _view?.Initialize();
    }

    private void OnEnable()
    {
        if (_model != null)
            _model.OnSlotChanged += OnSlotChanged;
        if (_view != null)
        {
            _view.OnDropEnded += HandleDropEnded;
            _view.OnUseItemRequested += HandleUseItemRequested;
            _view.OnRefreshRequested += RefreshView;
        }
        GameEvents.OnInventoryKeyPressed += HandleInventoryKeyPressed;
        RefreshView();
    }

    private void OnDisable()
    {
        GameEvents.OnInventoryKeyPressed -= HandleInventoryKeyPressed;
        if (_model != null)
            _model.OnSlotChanged -= OnSlotChanged;
        if (_view != null)
        {
            _view.OnDropEnded -= HandleDropEnded;
            _view.OnUseItemRequested -= HandleUseItemRequested;
            _view.OnRefreshRequested -= RefreshView;
        }
    }

    private void HandleUseItemRequested(int slotIndex)
    {
        if (_model != null)
            _model.TryUseItem(slotIndex);
    }

    private void HandleInventoryKeyPressed()
    {
        if (_view != null)
            _view.ToggleInventory();
    }

    private void OnSlotChanged(ItemSlotModel slot)
    {
        if (_view != null)
            _view.RefreshSlot(slot);
    }

    private void RefreshView()
    {
        if (_view == null || _model == null) return;
        var slots = _model.GetSlots();
        for (int i = 0; i < slots.Length; i++)
            _view.RefreshSlot(slots[i]);
    }

    private void HandleDropEnded(int fromIndex, Vector2 screenPosition)
    {
        if (_view == null || _model == null) return;
        int toIndex = _view.GetSlotIndexAtPosition(screenPosition);
        if (toIndex >= 0 && toIndex != fromIndex)
            _model.SwapItems(fromIndex, toIndex);
    }
}
