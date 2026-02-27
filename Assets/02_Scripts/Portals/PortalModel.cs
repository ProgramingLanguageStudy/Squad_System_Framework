using UnityEngine;

[System.Serializable]
public class PortalModel
{
    // --- 인스펙터 노출용 필드 ---
    [SerializeField] private Portal _portal;
    [SerializeField] private PortalData _data;
    [SerializeField] private Vector3 _worldPosition;
    [SerializeField] private Vector3 _arrivalPosition;
    [SerializeField] private bool _isUnlocked;

    // --- 외부 접근용 프로퍼티 ---
    public Portal Portal => _portal;
    public PortalData Data => _data;
    public Vector3 WorldPosition => _worldPosition;
    public Vector3 ArrivalPosition => _arrivalPosition;

    private FlagSystem _flagSystem;

    // IsUnlocked를 부를 때마다 FlagSystem에서 최신 상태를 읽어옴
    public bool IsUnlocked => _isUnlocked;

    public void SetUnlockState(bool isUnlocked)
    {
        _isUnlocked = isUnlocked;
    }

    public PortalModel(Portal portal, FlagSystem flagSystem)
    {
        _portal = portal;
        _data = portal.Data;
        _worldPosition = portal.transform.position;
        _arrivalPosition = portal.ArrivalPosition;
        _flagSystem = flagSystem;
    }
}