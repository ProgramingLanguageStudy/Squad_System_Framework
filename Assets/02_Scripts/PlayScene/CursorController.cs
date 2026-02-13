using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("UI 닫을 때 스핀 방지")]
    [Tooltip("닫을 때 커서를 화면 중앙으로 옮긴 뒤 잠금. Mouse.WarpCursorPosition 사용.")]
    [SerializeField] private bool _warpCursorToCenterOnHide = true;
    [Tooltip("닫은 뒤 카메라 회전을 몇 프레임 무시할지. 0=무시 안 함, 1~n=그만큼 지연 후 회전 켬. 테스트용으로 조절 가능.")]
    [SerializeField] [Min(0)] private int _ignoreLookFramesAfterLock = 2;

    private int _showRequestCount;
    private int _framesToIgnoreRemaining;

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

    private void Update()
    {
        if (_framesToIgnoreRemaining > 0)
        {
            _framesToIgnoreRemaining--;
            if (_framesToIgnoreRemaining == 0 && _cameraRotationInput != null)
                _cameraRotationInput.enabled = true;
        }
    }

    private void ApplyCursorState(bool visible)
    {
        if (!visible)
        {
            if (_warpCursorToCenterOnHide)
                WarpCursorToCenter();
            _framesToIgnoreRemaining = _ignoreLookFramesAfterLock;
        }

        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;

        if (_cameraRotationInput != null)
        {
            if (visible)
                _cameraRotationInput.enabled = false;
            else
                _cameraRotationInput.enabled = _ignoreLookFramesAfterLock == 0;
        }
    }

    private void WarpCursorToCenter()
    {
        if (Mouse.current != null)
            Mouse.current.WarpCursorPosition(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
    }
}
