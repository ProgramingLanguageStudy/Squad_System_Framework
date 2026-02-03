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
    [SerializeField] private QuestUI _questUI;

    private void Awake()
    {
        if (_input == null) _input = FindFirstObjectByType<InputHandler>();
        if (_player == null) _player = FindFirstObjectByType<Player>();
        if (_interactionUI == null) _interactionUI = FindFirstObjectByType<InteractionUI>();
        if (_flagManager == null) _flagManager = FindFirstObjectByType<FlagManager>();
        if (_dialogueUI == null) _dialogueUI = FindFirstObjectByType<DialogueUI>();
        if (_inventoryUI == null) _inventoryUI = FindFirstObjectByType<InventoryUI>();
        if (_questUI == null) _questUI = FindFirstObjectByType<QuestUI>();

        InitializeScene();
    }

    private void Start()
    {
        if (_input != null)
        {
            _input.OnInteractPerformed += HandleInteract;
            _input.OnInventoryPerformed += HandleInventory;
            _input.OnQuestPerformed += HandleQuest;
        }
    }

    private void Update()
    {
        bool isInventoryOpen = _inventoryUI != null && _inventoryUI.gameObject.activeSelf;
        bool isQuestOpen = _questUI != null && _questUI.IsOpen;
        bool isTalking = _dialogueUI != null && DialogueSystem.Instance.IsTalking;

        _player.CanMove = !(isTalking || isInventoryOpen || isQuestOpen);
    }

    private void InitializeScene()
    {
        Debug.Log("PlayScene: 시스템 초기화");

        _player.Initialize();
        _interactionUI.Setup();

        // 상호작용 타겟 변경 시 가이드 UI (예: "촌장과 대화하기[E]") 업데이트
        _player.Interactor.OnTargetChanged += (target) =>
        {
            UpdateInteractionUI(target);
        };

        // [중요] 대화가 끝나는 시점에 UI를 다시 계산하도록 이벤트 연결
        // DialogueSystem에 OnDialogueEnd 같은 Action이 있다면 연결하세요.
        DialogueSystem.Instance.OnDialogueEnd += () =>
        {
            // 대화가 끝나면 현재 바라보고 있는 타겟으로 UI 다시 갱신
            UpdateInteractionUI(_player.Interactor.CurrentTarget);
        };
    }

    // UI 갱신 로직을 별도 함수로 분리
    private void UpdateInteractionUI(IInteractable target)
    {
        bool isDialoguing = DialogueSystem.Instance.IsTalking; // IsActive 또는 IsTalking

        // 대화 중이 아니고 타겟이 있을 때만 텍스트 표시
        string text = (target != null && !isDialoguing) ? target.GetInteractText() : null;
        _interactionUI.Refresh(text);
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
        if (_dialogueUI != null && DialogueSystem.Instance.IsTalking) return;
        if (_inventoryUI != null)
        {
            _inventoryUI.ToggleInventory();
            if (_inventoryUI.gameObject.activeSelf)
                _interactionUI.Refresh(null);
        }
    }

    private void HandleQuest()
    {
        if (_dialogueUI != null && DialogueSystem.Instance.IsTalking) return;
        if (_questUI != null)
        {
            _questUI.Toggle();
            if (_questUI.IsOpen)
                _interactionUI.Refresh(null);
        }
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.OnInteractPerformed -= HandleInteract;
            _input.OnInventoryPerformed -= HandleInventory;
            _input.OnQuestPerformed -= HandleQuest;
        }
    }
}