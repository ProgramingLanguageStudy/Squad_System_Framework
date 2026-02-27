using System.Collections.Generic;
using UnityEngine;

public class PortalDebugger : MonoBehaviour
{
    [SerializeField] private PortalController _portalController;

    private void OnValidate()
    {
        if (_portalController == null)
            _portalController = FindAnyObjectByType<PortalController>();
    }

    public PortalController GetController()
    {
        if (_portalController == null)
            _portalController = FindAnyObjectByType<PortalController>();
        return _portalController;
    }

    // --- 실행 기능들 ---

    public void UnlockAll()
    {
        var pc = GetController();
        if (pc == null) return;

        foreach (var m in pc.PortalModels) m.SetUnlockState(true);
        Debug.Log("[PortalDebugger] 모든 포탈 해금 완료");
    }

    public void LockAll()
    {
        var pc = GetController();
        if (pc == null) return;

        foreach (var m in pc.PortalModels) m.SetUnlockState(false);
        Debug.Log("[PortalDebugger] 모든 포탈 잠금 완료");
    }

    public void TogglePortal(PortalModel model)
    {
        model.SetUnlockState(!model.IsUnlocked);
        Debug.Log($"[PortalDebugger] {model.Data.displayName} 상태: {model.IsUnlocked}");
    }
}