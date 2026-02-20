using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// 플레이 씬 조율. InputHandler·PlayerController 참조 보유, 입력 이벤트/값을 플레이어·GameEvents로 연결.
/// 플레이어는 PlayerController가 정함. 이동, Interact, 인벤토리 키만 연결.
/// </summary>
public class PlayScene : MonoBehaviour
{
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] [Tooltip("입력→월드 방향 변환에 사용. 비면 Camera.main")]
    private Transform _cameraTransform;
    [SerializeField] [Tooltip("비면 주입 안 함. 있으면 현재 조종 캐릭터를 chase target으로 주입")]
    private EnemySpawner _enemySpawner;
    [SerializeField] [Tooltip("비면 주입 안 함. 있으면 분대 스폰·따라가기 설정")]
    private SquadController _squadController;
    [SerializeField] [Tooltip("비면 주입 안 함. 있으면 PlayerController·조종 캐릭터 변경 시 ItemUser 갱신")]
    private InventoryPresenter _inventoryPresenter;
    [SerializeField] [Tooltip("비면 주입 안 함. 플레이 화면 UI(체력바 등). 조종 캐릭터 Model.OnHpChanged 구독")]
    private PlaySceneView _playSceneView;
    [SerializeField] [Tooltip("비면 갱신 안 함. 있으면 SquadSwap 시 Follow 타겟을 현재 조종 캐릭터로 변경")]
    private CinemachineCamera _cinemachineCamera;

    private CharacterModel _hpModelSubscribed;

    private void Awake()
    {
        if (_inputHandler == null)
        {
            Debug.LogWarning("[PlayScene] InputHandler가 할당되지 않았습니다. 인스펙터에서 할당해 주세요.");
            return;
        }
        if (_playerController == null)
        {
            Debug.LogWarning("[PlayScene] PlayerController가 할당되지 않았습니다.");
            return;
        }

        if (_cameraTransform == null && Camera.main != null)
            _cameraTransform = Camera.main.transform;

        _playerController.Initialize();

        var chaseTarget = _playerController.CurrentControlled != null ? _playerController.CurrentControlled.transform : transform;
        _enemySpawner?.Initialize(chaseTarget);
        if (_squadController != null)
        {
            _squadController.SetFollowTarget(chaseTarget);
            _squadController.Initialize();
        }

        if (_inventoryPresenter != null)
            _inventoryPresenter.SetPlayerController(_playerController);
    }

    private void OnEnable()
    {
        if (_inputHandler == null || _playerController == null) return;

        _playerController.OnCurrentControlledChanged += HandleCurrentControlledChanged;
        HandleCurrentControlledChanged(_playerController.CurrentControlled);

        _inputHandler.OnInteractPerformed += HandleInteract;
        _inputHandler.OnInventoryPerformed += HandleInventoryKey;
        _inputHandler.OnAttackPerformed += HandleAttack;
        _inputHandler.OnSquadSwapPerformed += HandleSquadSwap;
    }

    private void OnDisable()
    {
        if (_hpModelSubscribed != null)
        {
            _hpModelSubscribed.OnHpChanged -= OnHpChanged;
            _hpModelSubscribed = null;
        }
        if (_playerController != null)
            _playerController.OnCurrentControlledChanged -= HandleCurrentControlledChanged;
        if (_inputHandler == null) return;

        _inputHandler.OnInteractPerformed -= HandleInteract;
        _inputHandler.OnInventoryPerformed -= HandleInventoryKey;
        _inputHandler.OnAttackPerformed -= HandleAttack;
        _inputHandler.OnSquadSwapPerformed -= HandleSquadSwap;
    }

    private void Update()
    {
        if (_inputHandler == null || _playerController == null || _playerController.Mover == null) return;

        if (!_playerController.CanMove)
            return;

        Vector2 input = _inputHandler.MoveInput;
        Vector3 worldDir = InputToWorldDirection(input);
        _playerController.Mover.Move(worldDir);
    }

    /// <summary>입력 + 카메라 → 월드 기준 이동 방향.</summary>
    private Vector3 InputToWorldDirection(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f) return Vector3.zero;

        Transform cam = _cameraTransform != null ? _cameraTransform : Camera.main?.transform;
        if (cam == null) return Vector3.zero;

        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        return (forward * input.y + right * input.x).normalized;
    }

    private void HandleInteract()
    {
        _playerController?.Interactor?.TryInteract();
    }

    private void HandleInventoryKey()
    {
        GameEvents.OnInventoryKeyPressed?.Invoke();
    }

    private void HandleAttack()
    {
        _playerController?.RequestAttack();
    }

    private void HandleSquadSwap()
    {
        if (_playerController == null) return;
        _playerController.SwapSquad();
        // OnCurrentControlledChanged가 SetCurrentControlled에서 발행되어 HandleCurrentControlledChanged 호출
    }

    /// <summary>조종 캐릭터 변경 시 chase/follow/인벤토리/체력바/카메라 등 갱신. 새로 추가할 시스템은 여기에 한 줄 추가.</summary>
    private void HandleCurrentControlledChanged(Character newControlled)
    {
        var chaseTarget = newControlled != null ? newControlled.transform : _playerController?.transform;
        if (chaseTarget != null)
        {
            _enemySpawner?.SetChaseTarget(chaseTarget);
            _squadController?.SetFollowTarget(chaseTarget);
            if (_cinemachineCamera != null)
                _cinemachineCamera.Follow = chaseTarget;
        }
        _inventoryPresenter?.RefreshItemUser();

        if (_hpModelSubscribed != null)
            _hpModelSubscribed.OnHpChanged -= OnHpChanged;

        _hpModelSubscribed = newControlled?.Model;
        if (_hpModelSubscribed != null)
        {
            _hpModelSubscribed.OnHpChanged += OnHpChanged;
            _playSceneView?.RefreshHealth(_hpModelSubscribed.CurrentHp, _hpModelSubscribed.MaxHp);
        }
    }

    private void OnHpChanged(int currentHp, int maxHp)
    {
        _playSceneView?.RefreshHealth(currentHp, maxHp);
    }
}
