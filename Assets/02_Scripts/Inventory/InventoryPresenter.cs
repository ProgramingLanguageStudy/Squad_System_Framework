using UnityEngine;

/// <summary>
/// Inventory(Model)와 InventoryView(View)를 연결. Model·View는 인스펙터에서 할당. Model 변경 시 View 갱신, View에서 스왑 요청 시 Model 호출.
/// </summary>
public class InventoryPresenter : MonoBehaviour
{
    [SerializeField] private Inventory _model;
    [SerializeField] private InventoryView _view;

    private void Awake()
    {
        if (_model == null)
            Debug.LogWarning($"[InventoryPresenter] {gameObject.name}: Model(Inventory)이 할당되지 않았습니다. 인스펙터에서 할당해 주세요.");
        if (_view == null)
            Debug.LogWarning($"[InventoryPresenter] {gameObject.name}: View(InventoryView)가 할당되지 않았습니다. 인스펙터에서 할당해 주세요.");
    }

    private void OnEnable()
    {
        if (_model != null)
            _model.OnSlotChanged += OnSlotChanged;
        if (_view != null)
        {
            _view.OnSwapRequested += HandleSwapRequested;
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
            _view.OnSwapRequested -= HandleSwapRequested;
            _view.OnRefreshRequested -= RefreshView;
        }
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

    private void HandleSwapRequested(int indexA, int indexB)
    {
        if (_model != null)
            _model.SwapItems(indexA, indexB);
    }
}
