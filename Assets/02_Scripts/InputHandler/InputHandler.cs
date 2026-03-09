using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// PlayerInput이 참조하는 Input Action Asset을 사용. Cinemachine과 같은 인스턴스 공유 시 UI 모드에서 Look도 함께 꺼짐.
/// GameEvents OnCursorShowRequested/HideRequested로 blocking UI 개수 카운트 후, 1개 이상이면 Player 맵 비활성화(UI 모드).
/// Global.FreeCursor(Alt) started/canceled → 동일 이벤트 발행.
/// </summary>
public class InputHandler : MonoBehaviour
{
    // ── 의존성 ────────────────────────────────────────────────────────

    [Tooltip("PlayerInput 할당 필수. 같은 GameObject에 두거나, 에셋을 공유하는 PlayerInput을 넣으면 됨.")]
    [SerializeField] private PlayerInput _playerInput;

    // ── Asset & Maps ──────────────────────────────────────────────────

    private InputActionAsset _asset;
    private InputActionMap _playerMap;
    private InputActionMap _uiMap;
    private InputActionMap _globalMap;

    // ── Actions (Value 읽기용) ────────────────────────────────────────

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _uiScrollAction;
    private InputAction _uiInventoryAction;
    private InputAction _uiMapAction;
    private InputAction _freeCursorAction;

    // ── UI Mode ──────────────────────────────────────────────────────

    private int _blockingUICount;

    // ── Player performed 구독 해제용 ──────────────────────────────────

    private InputAction[] _playerButtonActions;
    private Action<InputAction.CallbackContext>[] _playerCallbacks;

    // ── Public ────────────────────────────────────────────────────────

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public Vector2 ScrollInput { get; private set; }

    /// <summary>true면 Player 액션 맵 비활성화(대화/인벤토리 등 UI 모드).</summary>
    public bool IsUIMode => _blockingUICount > 0;

    public event Action OnInteractPerformed;
    public event Action OnAttackPerformed;
    public event Action OnInventoryPerformed;
    public event Action OnSquadSwapPerformed;
    public event Action OnSavePerformed;
    public event Action OnMapPerformed;
    public event Action OnSettingsPerformed;

    // ── Unity ─────────────────────────────────────────────────────────

    private void OnEnable()
    {
        if (!ValidateAsset()) return;

        SetupPlayerMap();
        SetupUIMap();
        SetupGlobalMap();
        GameEvents.OnCursorShowRequested += OnBlockingUIOpened;
        GameEvents.OnCursorHideRequested += OnBlockingUIClosed;
    }

    private void OnDisable()
    {
        GameEvents.OnCursorShowRequested -= OnBlockingUIOpened;
        GameEvents.OnCursorHideRequested -= OnBlockingUIClosed;
        TeardownGlobalMap();
        TeardownUIMap();
        TeardownPlayerMap();
        ClearReferences();
    }

    private void Update()
    {
        if (_playerMap == null || !_playerMap.enabled)
        {
            MoveInput = Vector2.zero;
            LookInput = Vector2.zero;
        }
        if (_uiMap == null || !_uiMap.enabled)
            ScrollInput = Vector2.zero;

        MoveInput = _moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        LookInput = _lookAction?.ReadValue<Vector2>() ?? Vector2.zero;
        ScrollInput = _uiScrollAction?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    // ── Validate ──────────────────────────────────────────────────────

    private bool ValidateAsset()
    {
        if (_playerInput == null)
        {
            Debug.LogWarning("[InputHandler] PlayerInput이 할당되지 않았습니다. 인스펙터에서 PlayerInput을 할당해 주세요.");
            return false;
        }
        _asset = _playerInput.actions;
        if (_asset == null)
        {
            Debug.LogWarning("[InputHandler] PlayerInput.actions가 없습니다.");
            return false;
        }
        return true;
    }

    // ── Player Map ────────────────────────────────────────────────────

    private void SetupPlayerMap()
    {
        _playerMap = _asset.FindActionMap("Player");
        if (_playerMap == null)
        {
            Debug.LogWarning("[InputHandler] Player 액션 맵을 찾을 수 없습니다.");
            return;
        }

        _moveAction = _playerMap.FindAction("Move");
        _lookAction = _playerMap.FindAction("Look");

        var actionNames = new[] { "Interact", "Attack", "Inventory", "SquadSwap", "Save", "Map", "Settings" };
        var events = new Action[]
        {
            () => OnInteractPerformed?.Invoke(),
            () => OnAttackPerformed?.Invoke(),
            () => OnInventoryPerformed?.Invoke(),
            () => OnSquadSwapPerformed?.Invoke(),
            () => OnSavePerformed?.Invoke(),
            () => OnMapPerformed?.Invoke(),
            () => OnSettingsPerformed?.Invoke()
        };

        _playerButtonActions = new InputAction[actionNames.Length];
        _playerCallbacks = new Action<InputAction.CallbackContext>[actionNames.Length];

        for (int i = 0; i < actionNames.Length; i++)
        {
            var action = _playerMap.FindAction(actionNames[i]);
            if (action == null) continue;
            int index = i;
            var cb = (Action<InputAction.CallbackContext>)(_ => events[index]());
            _playerCallbacks[i] = cb;
            _playerButtonActions[i] = action;
            action.performed += cb;
        }

        _playerMap.Enable();
    }

    private void TeardownPlayerMap()
    {
        if (_playerMap == null) return;

        _playerMap.Disable();
        if (_playerButtonActions != null && _playerCallbacks != null)
        {
            for (int i = 0; i < _playerButtonActions.Length; i++)
            {
                var action = _playerButtonActions[i];
                var cb = _playerCallbacks[i];
                if (action != null && cb != null)
                    action.performed -= cb;
            }
        }
    }

    // ── UI Map ────────────────────────────────────────────────────────

    private void SetupUIMap()
    {
        _uiMap = _asset.FindActionMap("UI");
        if (_uiMap == null) return;

        _uiScrollAction = _uiMap.FindAction("ScrollWheel");
        _uiInventoryAction = _uiMap.FindAction("Inventory");
        _uiMapAction = _uiMap.FindAction("Map");

        var inventoryCb = (Action<InputAction.CallbackContext>)(_ => OnInventoryPerformed?.Invoke());
        var mapCb = (Action<InputAction.CallbackContext>)(_ => OnMapPerformed?.Invoke());

        if (_uiInventoryAction != null) _uiInventoryAction.performed += inventoryCb;
        if (_uiMapAction != null) _uiMapAction.performed += mapCb;

        _uiInventoryCallback = inventoryCb;
        _uiMapCallback = mapCb;
    }

    private Action<InputAction.CallbackContext> _uiInventoryCallback;
    private Action<InputAction.CallbackContext> _uiMapCallback;

    private void TeardownUIMap()
    {
        if (_uiMap == null) return;

        if (_uiInventoryAction != null && _uiInventoryCallback != null)
            _uiInventoryAction.performed -= _uiInventoryCallback;
        if (_uiMapAction != null && _uiMapCallback != null)
            _uiMapAction.performed -= _uiMapCallback;

        _uiMap.Disable();
    }

    // ── Global Map (FreeCursor) ────────────────────────────────────────

    private void SetupGlobalMap()
    {
        _globalMap = _asset.FindActionMap("Global");
        if (_globalMap == null) return;

        _freeCursorAction = _globalMap.FindAction("FreeCursor");
        if (_freeCursorAction == null) return;

        _globalMap.Enable();
        _freeCursorAction.started += OnFreeCursorStarted;
        _freeCursorAction.canceled += OnFreeCursorCanceled;
    }

    private void TeardownGlobalMap()
    {
        if (_freeCursorAction != null)
        {
            _freeCursorAction.started -= OnFreeCursorStarted;
            _freeCursorAction.canceled -= OnFreeCursorCanceled;
        }
        _globalMap?.Disable();
    }

    // ── 이벤트 핸들러 ──────────────────────────────────────────────────

    private void OnFreeCursorStarted(InputAction.CallbackContext _) => GameEvents.OnCursorShowRequested?.Invoke();
    private void OnFreeCursorCanceled(InputAction.CallbackContext _) => GameEvents.OnCursorHideRequested?.Invoke();

    private void OnBlockingUIOpened()
    {
        _blockingUICount++;
        if (_blockingUICount == 1)
            SwitchToUIMode();
    }

    private void OnBlockingUIClosed()
    {
        if (_blockingUICount > 0)
            _blockingUICount--;
        if (_blockingUICount == 0)
            SwitchToPlayMode();
    }

    // ── 모드 전환 ──────────────────────────────────────────────────────

    private void SwitchToUIMode()
    {
        _playerMap?.Disable();
        _uiMap?.Enable();
        MoveInput = Vector2.zero;
        LookInput = Vector2.zero;
    }

    private void SwitchToPlayMode()
    {
        _uiMap?.Disable();
        _playerMap?.Enable();
    }

    // ── 정리 ───────────────────────────────────────────────────────────

    private void ClearReferences()
    {
        _asset = null;
        _playerMap = null;
        _uiMap = null;
        _globalMap = null;
        _moveAction = null;
        _lookAction = null;
        _uiScrollAction = null;
        _uiInventoryAction = null;
        _uiMapAction = null;
        _freeCursorAction = null;
        _playerButtonActions = null;
        _playerCallbacks = null;
    }
}
