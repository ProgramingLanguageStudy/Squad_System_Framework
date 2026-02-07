using System;
using UnityEngine;

/// <summary>
/// 대화 독립 시스템. 하는 일만: 받은 대화 재생, 다음 문장, 닫기. 로딩·선택·버튼 종류는 모름.
/// </summary>
public class DialogueSystem : MonoBehaviour
{
    private readonly DialogueModel _model = new DialogueModel();
    private Action _onComplete;

    public DialogueModel Model => _model;
    public bool IsTalking => _model.IsTalking;
    public string CurrentSpeakerName => _model.CurrentSpeakerName;

    public event Action OnDialogueEnd
    {
        add => _model.OnDialogueEnd += value;
        remove => _model.OnDialogueEnd -= value;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayDialogueRequested += HandlePlayDialogueRequested;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayDialogueRequested -= HandlePlayDialogueRequested;
    }

    /// <summary>연결 포트. 외부가 DialogueData를 넘기면 재생만 함.</summary>
    private void HandlePlayDialogueRequested(DialogueData data)
    {
        if (data != null && !IsTalking)
            StartDialogue(data);
    }

    /// <summary>대화 시작. 받은 내용만 재생. 누가/어디서 왔는지는 모름.</summary>
    public void StartDialogue(DialogueData data, Action onComplete = null)
    {
        if (data == null) return;
        _onComplete = onComplete;
        _model.SetDialogue(data);
    }

    /// <summary>다음 문장. 끝이면 자동 종료.</summary>
    public void Next()
    {
        if (!_model.IsTalking) return;
        int next = _model.CurrentIndex + 1;
        if (next >= _model.LineCount)
        {
            _model.Clear();
            _onComplete?.Invoke();
            _onComplete = null;
        }
        else
        {
            _model.SetCurrentIndex(next);
        }
    }

    /// <summary>대화 강제 종료.</summary>
    public void EndDialogue()
    {
        if (!_model.IsTalking) return;
        _model.Clear();
        _onComplete?.Invoke();
        _onComplete = null;
    }
}
