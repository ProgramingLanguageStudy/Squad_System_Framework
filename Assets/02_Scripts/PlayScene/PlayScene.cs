using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [Header("----- 핵심 참조 -----")]
    [SerializeField] private InputHandler _input;
    [SerializeField] private Player _player;
    [SerializeField] private InteractionUI _interactionUI;
    [SerializeField] private InventoryUI _inventoryUI;
    [SerializeField] private DialogueUI _dialogueUI;
    [SerializeField] private FlagManager _flagManager;

    private void Awake()
    {
        // 1. 안전장치 및 자동 할당
        if (_input == null) _input = FindFirstObjectByType<InputHandler>();
        if (_player == null) _player = FindFirstObjectByType<Player>();
        if (_interactionUI == null) _interactionUI = FindFirstObjectByType<InteractionUI>();
        if (_flagManager == null) _flagManager = FindFirstObjectByType<FlagManager>();
        if (_dialogueUI == null) _dialogueUI = FindFirstObjectByType<DialogueUI>();
        if (_inventoryUI == null) _inventoryUI = FindFirstObjectByType<InventoryUI>();

        InitializeScene();
    }

    private void Start()
    {
        // 2. 이벤트 구독 (사건이 터졌을 때 실행할 함수 연결)
        if (_input != null)
        {
            _input.OnInteractPerformed += HandleInteract;
            _input.OnInventoryPerformed += HandleInventory;
        }
    }

    private void Update()
    {
        // 모든 상태를 체크해서 플레이어의 CanMove를 결정
        // 대화 중이거나, 인벤토리가 열려있으면 CanMove는 false!
        bool shouldBlock = _dialogueUI.IsActive || _inventoryUI.gameObject.activeSelf;

        _player.CanMove = !shouldBlock;
    }

    private void InitializeScene()
    {
        Debug.Log("PlayScene: 초기화를 시작합니다.");

        QuestManager.Instance.Setup(_flagManager);

        _player.Initialize();
        _interactionUI.Setup();

        // 상호작용 타겟 변경 시 가이드 UI 업데이트
        _player.Interactor.OnTargetChanged += (target) =>
        {
            string text = (target != null && !_dialogueUI.IsActive) ? target.GetInteractText() : null;
            _interactionUI.Refresh(text);
        };

        Debug.Log("PlayScene: 모든 시스템이 준비되었습니다.");
    }

    // --- 입력 처리 핸들러 ---

    private void HandleInteract()
    {
        // 1순위: 대화 중이면 다음 대사로
        if (_dialogueUI.IsActive)
        {
            DialogueSystem.Instance.DisplayNextSentence();
        }
        // 2순위: 인벤토리가 닫혀있을 때만 일반 상호작용 허용
        else if (!_inventoryUI.gameObject.activeSelf)
        {
            _player.Interactor.TryInteract();
        }
    }

    private void HandleInventory()
    {
        // 대화 중에는 인벤토리를 열 수 없도록 방어
        if (!_dialogueUI.IsActive)
        {
            _inventoryUI.ToggleInventory();

            if (_inventoryUI.gameObject.activeSelf)
            {
                _interactionUI.Refresh(null);
            }
        }
    }

    private void OnDestroy()
    {
        // 메모리 누수 방지를 위한 구독 해제
        if (_input != null)
        {
            _input.OnInteractPerformed -= HandleInteract;
            _input.OnInventoryPerformed -= HandleInventory;
        }
    }
}