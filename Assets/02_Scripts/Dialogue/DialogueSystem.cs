using System;
using UnityEngine;

public class DialogueSystem : Singleton<DialogueSystem>
{
    [SerializeField] private DialogueUI _ui;
    public bool IsTalking { get; private set; }
    public event Action OnDialogueEnd;

    /// <summary>퀘스트 수락 버튼 클릭 시 (npcId). 구독처에서 대사 전환·수락 처리.</summary>
    public event Action<string> OnQuestAcceptRequested;
    /// <summary>퀘스트 제출 버튼 클릭 시 (npcId). 구독처에서 제출·완료 대사 표시.</summary>
    public event Action<string> OnQuestSubmitRequested;

    private Action _onDialogueComplete;
    private Action _onDialogueEndOnce;
    private string _currentSpeakerName;
    private string _currentNpcId;
    private DialogueType _currentDialogueType;
    private bool _questButtonIsSubmit;

    public string CurrentNpcId => _currentNpcId;
    public string CurrentSpeakerName => _currentSpeakerName;
    public DialogueType CurrentDialogueType => _currentDialogueType;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>다음 대화 종료 시 한 번만 호출될 콜백 등록. (퀘스트 수락 시 플래그·수락 처리용)</summary>
    public void RegisterOnDialogueEndOnce(Action callback)
    {
        _onDialogueEndOnce = callback;
    }

    /// <summary>대사 내용만 바꿀 때 (퀘스트 수락/제출 처리 쪽에서 호출)</summary>
    public void ReplaceContent(string speakerName, string[] sentences)
    {
        _ui.ReplaceContent(speakerName, sentences);
    }

    public void SetQuestButtonVisible(bool visible)
    {
        _ui.SetQuestButtonVisible(visible);
    }

    public void StartDialogue(string speakerName, string[] sentences, string npcId, DialogueType dialogueType,
        bool showQuestButton, string questButtonText, Action onComplete = null,
        bool showSubmitButton = false, string submitButtonText = "제출하기")
    {
        if (IsTalking) return;
        IsTalking = true;
        _onDialogueComplete = onComplete;
        _onDialogueEndOnce = null;
        _currentSpeakerName = speakerName;
        _currentNpcId = npcId;
        _currentDialogueType = dialogueType;
        _questButtonIsSubmit = showSubmitButton;
        _ui.Open(speakerName, sentences, showQuestButton, questButtonText, showSubmitButton, submitButtonText);
    }

    public void DisplayNextSentence()
    {
        if (!IsTalking) return;
        _ui.ShowNext();
    }

    public void EndDialogue()
    {
        if (!IsTalking) return;
        IsTalking = false;
        _ui.Close();
        OnDialogueEnd?.Invoke();
        _onDialogueComplete?.Invoke();
        _onDialogueComplete = null;
        _onDialogueEndOnce?.Invoke();
        _onDialogueEndOnce = null;
        _currentSpeakerName = null;
        _currentNpcId = null;
        _currentDialogueType = DialogueType.Common;
    }

    /// <summary>퀘스트 버튼 클릭 시: 수락/제출 이벤트만 발행. 실제 처리는 구독처(QuestManager)에서.</summary>
    public void OnQuestPanelButtonClicked()
    {
        if (string.IsNullOrEmpty(_currentNpcId)) return;
        if (_questButtonIsSubmit)
            OnQuestSubmitRequested?.Invoke(_currentNpcId);
        else
            OnQuestAcceptRequested?.Invoke(_currentNpcId);
    }
}
