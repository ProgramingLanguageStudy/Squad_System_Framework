using System.Linq;
using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [Header("----- 핵심 참조 -----")]
    [SerializeField] private InputHandler _input;
    [SerializeField] private Player _player;
    [SerializeField] private InteractionUI _interactionUI;
    [SerializeField] private InventoryView _inventoryView;
    [SerializeField] private InventoryPresenter _inventoryPresenter;
    [SerializeField] private DialogueUI _dialogueUI;
    [SerializeField] private FlagManager _flagManager;
    [SerializeField] private Inventory _inventory;

    [Header("----- 퀘스트 (MVP) -----")]
    [SerializeField] private QuestModel _questModel;
    [SerializeField] private QuestView _questView;
    [SerializeField] private QuestPresenter _questPresenter;

    [Header("----- 대화 조율 -----")]
    [SerializeField] private DialogueCoordinator _dialogueCoordinator;

    private void Awake()
    {
        if (_input == null) _input = FindFirstObjectByType<InputHandler>();
        if (_player == null) _player = FindFirstObjectByType<Player>();
        if (_interactionUI == null) _interactionUI = FindFirstObjectByType<InteractionUI>();
        if (_inventoryView == null) _inventoryView = FindFirstObjectByType<InventoryView>();
        if (_inventoryPresenter == null) _inventoryPresenter = FindFirstObjectByType<InventoryPresenter>();
        if (_dialogueUI == null) _dialogueUI = FindFirstObjectByType<DialogueUI>();
        if (_flagManager == null) _flagManager = FindFirstObjectByType<FlagManager>();
        if (_inventory == null) _inventory = FindFirstObjectByType<Inventory>();
        if (_questModel == null) _questModel = FindFirstObjectByType<QuestModel>();
        if (_questView == null) _questView = FindFirstObjectByType<QuestView>();
        if (_questPresenter == null) _questPresenter = FindFirstObjectByType<QuestPresenter>();
        if (_dialogueCoordinator == null) _dialogueCoordinator = FindFirstObjectByType<DialogueCoordinator>();

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
        // 플레이 중 기본: 커서 숨김
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        bool isInventoryOpen = _inventoryView != null && _inventoryView.IsPanelActive;
        bool isTalking = _dialogueUI != null && DialogueSystem.Instance.IsTalking;
        // 퀘스트 트래커는 항상 표시되므로 이동 제한에서 제외

        _player.CanMove = !(isTalking || isInventoryOpen);

        // 대화 중이거나 UI(인벤토리 등)를 쓰는 동안만 커서 표시
        bool needCursor = isTalking || isInventoryOpen;
        if (needCursor)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void InitializeScene()
    {
        Debug.Log("PlayScene: 시스템 초기화");

        _player.Initialize();
        _interactionUI.Setup();

        if (_inventory != null)
            _inventory.OnItemChangedWithId += OnInventoryItemChanged;

        if (_dialogueCoordinator != null && DialogueManager.Instance != null && DialogueSystem.Instance != null && _questModel != null && _flagManager != null)
            _dialogueCoordinator.Initialize(DialogueManager.Instance, DialogueSystem.Instance, _questModel, _flagManager);

        _player.Interactor.OnTargetChanged += (target) => UpdateInteractionUI(target);
        DialogueSystem.Instance.OnDialogueEnd += OnDialogueEnded;

        var ds = DialogueSystem.Instance;
        if (ds != null && _questModel != null)
        {
            ds.OnQuestAcceptRequested += HandleQuestAcceptRequested;
            ds.OnQuestCompleteRequested += HandleQuestCompleteRequested;
        }
    }

    /// <summary>대화창에서 퀘스트 수락 버튼 클릭 시. 대사 전환 후 대화 종료 시 수락·플래그·수집 소급 적용.</summary>
    private void HandleQuestAcceptRequested(string npcId)
    {
        var questDialogue = DialogueManager.Instance.GetQuestDialogue(npcId);
        if (questDialogue == null || string.IsNullOrEmpty(questDialogue.LinkedQuestId)) return;

        string[] sentences = questDialogue.Sentence.Split('/').Select(s => s.Trim()).ToArray();
        var ds = DialogueSystem.Instance;
        ds.ReplaceContent(ds.CurrentSpeakerName, sentences);
        ds.SetQuestButtonVisible(false);

        ds.RegisterOnDialogueEndOnce(() =>
        {
            if (_flagManager != null)
                _flagManager.SetFlag(GameStateKeys.QuestAccepted(questDialogue.LinkedQuestId), 1);
            var questData = Resources.Load<QuestData>($"Quests/{questDialogue.LinkedQuestId}");
            if (questData == null || _questModel == null) return;

            _questModel.AcceptQuest(questData);
            // 수집 퀘스트: 인벤토리 현재 개수로 진행도 동기화 (퀘스트 시스템은 인벤토리를 모름 → 조율에서 처리)
            if (_inventory != null)
            {
                foreach (var task in questData.Tasks)
                {
                    if (task is GatherTask gatherTask)
                        _questModel.SetGatherProgress(questData.QuestId, gatherTask.TargetItemId, _inventory.GetTotalCount(gatherTask.TargetItemId));
                }
            }
        });
    }

    /// <summary>대화창에서 퀘스트 완료 버튼 클릭 시. 아이템 차감·플래그는 여기서, 목록 제거만 QuestModel에 위임.</summary>
    private void HandleQuestCompleteRequested(string npcId)
    {
        if (!QuestDialogueQueries.GetCompletableQuestForNpc(DialogueManager.Instance, _questModel, npcId, out var quest, out var completionDialogue, out _))
            return;
        if (_questModel == null) return;

        if (_inventory != null)
        {
            foreach (var task in quest.Tasks)
            {
                if (task.RequiresItemDeduction && !_inventory.RemoveItem(task.TargetId, task.TargetAmount))
                    return;
            }
        }

        _questModel.CompleteQuest(quest.QuestId);
        if (_flagManager != null)
            _flagManager.SetFlag(GameStateKeys.QuestCompleted(quest.QuestId), 1);

        string[] sentences = completionDialogue.Sentence.Split('/').Select(s => s.Trim()).ToArray();
        var ds = DialogueSystem.Instance;
        ds.ReplaceContent(ds.CurrentSpeakerName, sentences);
        ds.SetQuestButtonVisible(false);
    }

    private void OnDialogueEnded()
    {
        UpdateInteractionUI(_player.Interactor.CurrentTarget);
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
        // 대화 등 상호작용이 시작되면 InteractionUI(예: "[E] 대화하기") 숨김
        UpdateInteractionUI(_player.Interactor.CurrentTarget);
    }

    private void HandleInventory()
    {
        if (_dialogueUI != null && DialogueSystem.Instance.IsTalking) return;
        if (_inventoryView != null)
        {
            _inventoryView.ToggleInventory();
            if (_inventoryView.IsPanelActive)
                _interactionUI.Refresh(null);
        }
    }

    private void HandleQuest()
    {
        if (_dialogueUI != null && DialogueSystem.Instance.IsTalking) return;
        // TODO: 나중에 전체 퀘스트 목록 UI 열기/닫기 (현재 퀘스트 트래커는 항상 표시됨)
    }

    private void OnInventoryItemChanged(string itemId, int totalCount)
    {
        GameEvents.OnQuestGoalProcessed?.Invoke(itemId, totalCount);
    }

    private void OnDestroy()
    {
        if (_inventory != null)
            _inventory.OnItemChangedWithId -= OnInventoryItemChanged;
        if (_input != null)
        {
            _input.OnInteractPerformed -= HandleInteract;
            _input.OnInventoryPerformed -= HandleInventory;
            _input.OnQuestPerformed -= HandleQuest;
        }
        var ds = DialogueSystem.Instance;
        if (ds != null)
        {
            ds.OnDialogueEnd -= OnDialogueEnded;
            ds.OnQuestAcceptRequested -= HandleQuestAcceptRequested;
            ds.OnQuestCompleteRequested -= HandleQuestCompleteRequested;
        }
    }
}