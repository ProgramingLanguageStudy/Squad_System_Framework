using System;
using UnityEngine;

/// <summary>
/// 대화 진입점(파사드). Model 보유, 외부는 StartDialogue/DisplayNextSentence/EndDialogue만 호출.
/// Presenter가 Model·View를 연결함.
/// </summary>
public class DialogueSystem : Singleton<DialogueSystem>
{
    private readonly DialogueModel _model = new DialogueModel();

    public DialogueModel Model => _model;

    public bool IsTalking => _model.IsTalking;
    public string CurrentNpcId => _model.CurrentNpcId;
    public string CurrentSpeakerName => _model.CurrentSpeakerName;
    public DialogueType CurrentDialogueType => _model.CurrentDialogueType;

    public event Action OnDialogueEnd
    {
        add => _model.OnDialogueEnd += value;
        remove => _model.OnDialogueEnd -= value;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void StartDialogue(string speakerName, string[] sentences, string npcId, DialogueType dialogueType, Action onComplete = null)
    {
        _model.StartDialogue(speakerName, sentences, npcId, dialogueType, onComplete);
    }

    /// <summary>E키 등으로 "다음" 요청 시. Presenter가 Model 갱신 시 View 자동 반영.</summary>
    public void DisplayNextSentence()
    {
        if (!_model.IsTalking) return;
        if (_model.AdvanceNext())
            _model.EndDialogue();
    }

    public void EndDialogue()
    {
        _model.EndDialogue();
    }

    public void ReplaceContent(string speakerName, string[] sentences)
    {
        _model.ReplaceContent(speakerName, sentences);
    }

    /// <summary>퀘스트 버튼 표시. Presenter 경유로 View에 전달하려면 Presenter.SetQuestButtonVisible 사용.</summary>
    public void SetQuestButtonVisible(bool visible)
    {
        var presenter = FindFirstObjectByType<DialoguePresenter>();
        if (presenter != null)
            presenter.SetQuestButtonVisible(visible);
    }

    /// <summary>퀘스트 버튼 클릭 시. 연결 시 구독처에서 처리.</summary>
    public void OnQuestPanelButtonClicked()
    {
        // 퀘스트 연동 시 이벤트 발행 등
    }
}
