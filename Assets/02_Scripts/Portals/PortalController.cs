using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 맵의 포탈을 참조로 보유. 각 포탈의 OnInteracted에 구독해 '갈 수 있는 포탈 목록'을 PortalMenuView로 넘김.
/// 해금 로직은 나중에 여기서만 관리하면 됨.
/// </summary>
public class PortalController : MonoBehaviour
{
    [SerializeField] [Tooltip("이 맵에 있는 포탈 전부. 인스펙터에서 할당")]
    private List<Portal> _portals = new List<Portal>();

    [SerializeField] [Tooltip("포탈 선택 메뉴. 여기서 직접 Show 호출")]
    private PortalMenuView _menuView;

    private void OnEnable()
    {
        for (int i = 0; i < _portals.Count; i++)
        {
            if (_portals[i] != null)
                _portals[i].OnInteracted += HandlePortalInteracted;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _portals.Count; i++)
        {
            if (_portals[i] != null)
                _portals[i].OnInteracted -= HandlePortalInteracted;
        }
    }

    private void HandlePortalInteracted(IInteractReceiver receiver, Portal interactedPortal)
    {
        if (receiver == null || interactedPortal == null) return;

        var reachable = GetReachablePortals(interactedPortal);
        _menuView?.Show(reachable, receiver);
    }

    /// <summary>
    /// 현재 갈 수 있는 포탈 목록. 기본: 상호작용한 포탈 제외한 전부. 나중에 해금 조건 추가.
    /// </summary>
    private IReadOnlyList<Portal> GetReachablePortals(Portal currentPortal)
    {
        var list = new List<Portal>(_portals.Count);
        for (int i = 0; i < _portals.Count; i++)
        {
            var p = _portals[i];
            if (p == null || p == currentPortal) continue;
            // TODO: 해금 여부 체크
            list.Add(p);
        }
        return list;
    }
}
