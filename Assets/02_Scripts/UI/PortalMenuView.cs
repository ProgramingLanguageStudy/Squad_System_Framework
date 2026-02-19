using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 포탈 선택 메뉴 View. Panel + PortalSlot 프리팹으로 목록 표시. PortalController가 참조해 직접 Show 호출.
/// </summary>
public class PortalMenuView : PanelViewBase
{
    [SerializeField] [Tooltip("띄울 패널 루트")]
    private GameObject _panel;

    [SerializeField] [Tooltip("슬롯이 생성될 부모 (Content 등)")]
    private Transform _slotContainer;

    [SerializeField] [Tooltip("PortalSlot 컴포넌트가 붙은 프리팹")]
    private PortalSlot _slotPrefab;

    private void Awake()
    {
        if (_panel != null)
            _panel.SetActive(false);
    }

    /// <summary>
    /// 포탈 목록으로 패널을 띄우고, 슬롯 생성해 이름 표시. 슬롯 클릭 시 해당 포탈로 Teleport 후 패널 닫음.
    /// </summary>
    public void Show(IReadOnlyList<Portal> portals, IInteractReceiver receiver)
    {
        if (_panel == null || _slotContainer == null || _slotPrefab == null || receiver == null)
            return;

        ClearSlots();

        for (int i = 0; i < portals.Count; i++)
        {
            Portal portal = portals[i];
            if (portal == null) continue;

            Portal captured = portal;
            PortalSlot slot = Instantiate(_slotPrefab, _slotContainer);
            slot.Set(portal.DisplayName, () =>
            {
                receiver.Teleport(captured.ArrivalPosition);
                Hide();
            });
        }

        OpenPanel();
    }

    /// <summary> 패널만 숨김. </summary>
    public void Hide()
    {
        ClosePanel();
    }

    protected override void OnPanelOpened()
    {
        if (_panel != null) _panel.SetActive(true);
    }

    protected override void OnPanelClosed()
    {
        if (_panel != null) _panel.SetActive(false);
    }

    private void ClearSlots()
    {
        if (_slotContainer == null) return;
        for (int i = _slotContainer.childCount - 1; i >= 0; i--)
            Destroy(_slotContainer.GetChild(i).gameObject);
    }
}
