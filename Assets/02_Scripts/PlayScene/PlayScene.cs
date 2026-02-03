using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [Header("----- 핵심 참조 -----")]
    [SerializeField] private InputHandler _input;
    [SerializeField] private Player _player;
    [SerializeField] private InteractionUI _interactionUI;
    [SerializeField] private InventoryUI _inventoryUI;

    // DialogueUI 대신 실제 로직을 담당하는 DialogueSystem을 참조하는 것이 좋습니다.
    [SerializeField] private DialogueUI _dialogueUI;
    [SerializeField] private FlagManager _flagManager;

    private void Awake()
    {
        // 1. 자동 할당 (없을 경우에만)
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
        // 2. 이벤트 구독 (New Input System 기반)
        if (_input != null)
        {
            _input.OnInteractPerformed += HandleInteract;
            _input.OnInventoryPerformed += HandleInventory;
        }
    }

    private void Update()
    {
        // 대화 중이거나 인벤토리가 열려있으면 플레이어 이동 제한
        // DialogueSystem의 상태를 체크하는 프로퍼티가 있다면 그걸 사용하세요.
        bool isInventoryOpen = _inventoryUI != null && _inventoryUI.gameObject.activeSelf;

        _player.CanMove = !(_dialogueUI != null && DialogueSystem.Instance.IsTalking || isInventoryOpen);

        Debug.Log($"{_dialogueUI != null && DialogueSystem.Instance.IsTalking}");
    }

    private void InitializeScene()
    {
        Debug.Log("PlayScene: 시스템 초기화");

        // 퀘스트 매니저에 플래그 매니저 전달
        QuestManager.Instance.Setup(_flagManager);

        _player.Initialize();
        _interactionUI.Setup();

        // 상호작용 타겟 변경 시 가이드 UI (예: "촌장과 대화하기[E]") 업데이트
        _player.Interactor.OnTargetChanged += (target) =>
        {
            // 대화 중이 아닐 때만 상호작용 텍스트 표시
            string text = (target != null && !(_dialogueUI != null && DialogueSystem.Instance.IsTalking)) ? target.GetInteractText() : null;
            _interactionUI.Refresh(text);
        };
    }

    // --- 입력 처리 핸들러 (방아쇠) ---

    private void HandleInteract()
    {
        // 1순위: 대화가 진행 중이라면 '다음 문장'으로 넘김
        if (_dialogueUI != null && DialogueSystem.Instance.IsTalking)
        {
            DialogueSystem.Instance.DisplayNextSentence();
            return; // 다음 로직 실행 방지
        }

        _player.Interactor.TryInteract();
    }

    private void HandleInventory()
    {
        // 대화 중에는 인벤토리 조작 금지
        if (_dialogueUI != null && DialogueSystem.Instance.IsTalking) return;

        if (_inventoryUI != null)
        {
            _inventoryUI.ToggleInventory();

            // 인벤토리를 열면 상호작용 가이드 UI는 숨김
            if (_inventoryUI.gameObject.activeSelf)
            {
                _interactionUI.Refresh(null);
            }
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제 (메모리 누수 및 에러 방지)
        if (_input != null)
        {
            _input.OnInteractPerformed -= HandleInteract;
            _input.OnInventoryPerformed -= HandleInventory;
        }
    }
}