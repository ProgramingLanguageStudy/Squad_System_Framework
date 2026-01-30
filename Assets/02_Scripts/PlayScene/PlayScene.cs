using UnityEngine;

public class PlayScene : MonoBehaviour
{
    [Header("----- 핵심 참조 -----")]
    [SerializeField] private Player _player;
    [SerializeField] private InteractionUI _interactionUI;

    // --- 새로 추가될 참조들 ---
    [SerializeField] private FlagManager _flagManager;
    [SerializeField] private DialogueSystem _dialogueSystem;
    [SerializeField] private DialogueUI _dialogueUI;

    private void Awake()
    {
        // 1. 안전장치 (새로운 시스템들 추가)
        if (_player == null) _player = FindFirstObjectByType<Player>();
        if (_interactionUI == null) _interactionUI = FindFirstObjectByType<InteractionUI>();
        if (_flagManager == null) _flagManager = FindFirstObjectByType<FlagManager>();
        if (_dialogueSystem == null) _dialogueSystem = FindFirstObjectByType<DialogueSystem>();
        if (_dialogueUI == null) _dialogueUI = FindFirstObjectByType<DialogueUI>();

        InitializeScene();
    }

    private void InitializeScene()
    {
        Debug.Log("PlayScene: 초기화를 시작합니다.");

        // 1. 시스템 간의 의존성 연결 (핵심!)
        _dialogueSystem.Setup(_flagManager);

        // 2. Player 및 UI 초기화
        _player.Initialize();
        _interactionUI.Setup();

        // 3. 상호작용 및 대화 입력 처리
        _player.Interactor.OnTargetChanged += (target) =>
        {
            // 대화 중일 때는 상호작용 가이드를 띄우지 않도록 조건 추가 가능
            string text = (target != null && !_dialogueUI.IsActive) ? target.GetInteractText() : null;
            _interactionUI.Refresh(text);
        };

        Debug.Log("PlayScene: 모든 시스템이 준비되었습니다.");
    }
}