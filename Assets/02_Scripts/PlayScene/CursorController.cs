using UnityEngine;

/// <summary>
/// 커서 표시/숨김을 한 곳에서만 처리. GameEvents OnCursorShowRequested/OnCursorHideRequested를 ref count로 구독.
/// 요청이 하나라도 있으면 보이고, 모두 해제되면 숨김. 플레이 기본값은 숨김 + Locked.
/// 플레이어 입력(이동·공격 등) 전환은 InputHandler가 같은 이벤트로 Play/UI 맵 전환 처리.
/// </summary>
public class CursorController : MonoBehaviour
{
    [Header("UI 열릴 때 끌 것 (선택)")]
    [Tooltip("카메라 회전 입력 담당. 예: CinemachineInputProvider. 비워두면 회전만 그대로 둠.")]
    [SerializeField] private MonoBehaviour _cameraRotationInput;

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

        if (_cameraRotationInput != null)
            _cameraRotationInput.enabled = !visible;
    }
}
