using UnityEngine;

/// <summary>
/// 플레이어 = 컴포넌트 조합부. 필요한 컴포넌트는 [SerializeField]로 인스펙터에서 참조하고,
/// Initialize() 시 각 컴포넌트에 필요한 의존성만 주입한다.
/// </summary>
[RequireComponent(typeof(PlayerModel)), RequireComponent(typeof(PlayerMover)), RequireComponent(typeof(PlayerAnimator)), RequireComponent(typeof(PlayerInteractor)), RequireComponent(typeof(PlayerAttacker)), RequireComponent(typeof(PlayerStateMachine))]
public class Player : MonoBehaviour
{
    [Header("----- 부품 (인스펙터에서 연결) -----")]
    [SerializeField] private PlayerModel _model;
    [SerializeField] private PlayerMover _mover;
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private PlayerInteractor _interactor;
    [SerializeField] private PlayerAttacker _attacker;
    [SerializeField] private PlayerStateMachine _stateMachine;
    [SerializeField] [Tooltip("체력바 UI. 있으면 Initialize 시 Model 주입")]
    private PlayerHealthBarView _healthBarView;
    [SerializeField] [Tooltip("세이브/로드 시 DataManager에 등록. 있으면 Initialize 시 Player 주입")]
    private PlayerSaveHandler _saveHandler;

    [Header("----- 주입용 참조 (비어 있으면 같은 GameObject에서 자동 탐색) -----")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _mainCameraTransform;

    public PlayerModel Model => _model;
    public PlayerMover Mover => _mover;
    public PlayerAnimator Animator => _playerAnimator;
    public PlayerInteractor Interactor => _interactor;
    public PlayerAttacker Attacker => _attacker;
    public PlayerStateMachine StateMachine => _stateMachine;

    public bool CanMove { get; set; } = true;

    /// <summary>공격 입력 시 호출. 조건(Idle 등)은 상태머신이 판단하고, 가능하면 Attack 상태로 전환.</summary>
    public void RequestAttack()
    {
        _stateMachine?.RequestAttack();
    }

    /// <summary>지정 위치로 순간이동. Portal·부활 등에서 호출. CharacterController 안전 처리 포함.</summary>
    public void Teleport(Vector3 worldPosition)
    {
        if (_characterController == null)
        {
            transform.position = worldPosition;
            return;
        }
        _characterController.enabled = false;
        transform.position = worldPosition;
        _characterController.enabled = true;
    }

    /// <summary>지정 Transform 위치로 순간이동. (회전은 적용하지 않음)</summary>
    public void Teleport(Transform destination)
    {
        if (destination == null) return;
        Teleport(destination.position);
    }

    /// <summary>CharacterController 켜기/끄기. Dead 시 끄면 몬스터 감지·공격 대상에서 제외.</summary>
    public void SetCharacterControllerEnabled(bool enabled)
    {
        if (_characterController != null)
            _characterController.enabled = enabled;
    }

    public void Initialize()
    {
        // 인스펙터 미연결 시 같은 GameObject에서 한 번만 보충 (선택)
        if (_model == null) _model = GetComponent<PlayerModel>();
        if (_mover == null) _mover = GetComponent<PlayerMover>();
        if (_playerAnimator == null) _playerAnimator = GetComponent<PlayerAnimator>();
        if (_interactor == null) _interactor = GetComponent<PlayerInteractor>();
        if (_attacker == null) _attacker = GetComponent<PlayerAttacker>();
        if (_stateMachine == null) _stateMachine = GetComponent<PlayerStateMachine>();
        if (_healthBarView == null) _healthBarView = GetComponentInChildren<PlayerHealthBarView>(true);
        if (_characterController == null) _characterController = GetComponent<CharacterController>();
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_mainCameraTransform == null && Camera.main != null) _mainCameraTransform = Camera.main.transform;
        if (_saveHandler == null) _saveHandler = GetComponent<PlayerSaveHandler>();
        if (_saveHandler != null) _saveHandler.SetPlayer(this);

        _model?.Initialize();
        _mover.Initialize(_characterController, _mainCameraTransform, _model);
        _playerAnimator.Initialize(_animator, _mover);
        _interactor.Initialize(this);
        _stateMachine.Initialize(this);
        _attacker.Initialize(_stateMachine, _model);
        _healthBarView?.Initialize(_model);
    }

    /// <summary>현재 위치·회전·체력을 PlayerSaveData로 반환. PlayerSaveHandler의 Gather에서 사용.</summary>
    public PlayerSaveData GetSaveData()
    {
        var d = new PlayerSaveData();
        d.position = transform.position;
        d.rotationY = transform.eulerAngles.y;
        d.currentHp = _model != null ? _model.CurrentHp : 0;
        return d;
    }

    /// <summary>로드한 PlayerSaveData를 실제 파츠에 적용. PlayerSaveHandler의 Apply에서 사용.</summary>
    public void ApplySaveData(PlayerSaveData data)
    {
        if (data == null) return;
        Teleport(data.position);
        transform.eulerAngles = new Vector3(0f, data.rotationY, 0f);
        _model?.SetCurrentHpForLoad(data.currentHp);
    }
}
