using UnityEngine;

/// <summary>
/// 커서 표시/숨김을 한 곳에서만 처리. GameEvents OnCursorShowRequested/OnCursorHideRequested를 ref count로 구독.
/// 요청이 하나라도 있으면 보이고, 모두 해제되면 숨김. 플레이 기본값은 숨김 + Locked.
/// UI 모드일 때 카메라 회전 입력·플레이어 이동도 함께 끔(인스펙터에서 선택 할당).
/// </summary>
public class CursorController : MonoBehaviour
{
    [Header("UI 열릴 때 끌 것 (선택)")]
    [Tooltip("카메라 회전 입력 담당 컴포넌트. 예: CinemachineInputProvider. 비워두면 회전만 그대로 둠.")]
    [SerializeField] private MonoBehaviour _cameraRotationInput;

    [Tooltip("플레이어. 비워두면 이동만 그대로 둠.")]
    [SerializeField] private Player _player;

    private int _showRequestCount;

    private void Start()
    {
        ApplyCursorState(false);
    }

    private void OnEnable()
    {
        GameEvents.OnCursorShowRequested += HandleShowRequested;
        GameEvents.OnCursorHideRequested += HandleHideRequested;
    }

    private void OnDisable()
    {
        GameEvents.OnCursorShowRequested -= HandleShowRequested;
        GameEvents.OnCursorHideRequested -= HandleHideRequested;
    }

    private void HandleShowRequested()
    {
        _showRequestCount++;
        ApplyCursorState(true);
    }

    private void HandleHideRequested()
    {
        if (_showRequestCount > 0)
            _showRequestCount--;
        ApplyCursorState(_showRequestCount > 0);
    }

    private void ApplyCursorState(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;

        // UI 열려 있을 때는 카메라 회전·플레이어 이동 끔
        if (_cameraRotationInput != null)
            _cameraRotationInput.enabled = !visible;
        if (_player != null)
            _player.CanMove = !visible;
    }
}
