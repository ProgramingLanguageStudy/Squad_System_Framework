using System;

/// <summary>
/// 대화 상태·로직 (MVP의 Model). 문장 목록, 현재 인덱스, 진행/종료. View·다른 시스템을 모름.
/// </summary>
public class DialogueModel
{
    private string _speakerName;
    private string[] _sentences;
    private int _currentIndex;
    private string _npcId;
    private DialogueType _dialogueType;
    private Action _onComplete;

    public bool IsTalking { get; private set; }
    public string CurrentNpcId => _npcId;
    public string CurrentSpeakerName => _speakerName;
    public DialogueType CurrentDialogueType => _dialogueType;

    /// <summary>상태가 바뀔 때 (시작/다음/종료). Presenter가 구독해 View 갱신.</summary>
    public event Action OnDialogueStateChanged;
    /// <summary>대화가 종료될 때. 조율층 등이 구독.</summary>
    public event Action OnDialogueEnd;

    public void StartDialogue(string speakerName, string[] sentences, string npcId, DialogueType dialogueType, Action onComplete = null)
    {
        if (IsTalking) return;
        _speakerName = speakerName ?? "";
        _sentences = sentences != null && sentences.Length > 0 ? sentences : Array.Empty<string>();
        _currentIndex = 0;
        _npcId = npcId ?? "";
        _dialogueType = dialogueType;
        _onComplete = onComplete;
        IsTalking = true;
        OnDialogueStateChanged?.Invoke();
    }

    /// <summary>다음 문장으로. true면 대화 끝(더 없음).</summary>
    public bool AdvanceNext()
    {
        if (!IsTalking || _sentences == null) return true;
        _currentIndex++;
        if (_currentIndex >= _sentences.Length)
            return true;
        OnDialogueStateChanged?.Invoke();
        return false;
    }

    public void EndDialogue()
    {
        if (!IsTalking) return;
        IsTalking = false;
        _onComplete?.Invoke();
        _onComplete = null;
        _speakerName = null;
        _sentences = null;
        _currentIndex = 0;
        _npcId = null;
        _dialogueType = DialogueType.Common;
        OnDialogueEnd?.Invoke();
        OnDialogueStateChanged?.Invoke();
    }

    public string GetCurrentSentence()
    {
        if (_sentences == null || _currentIndex < 0 || _currentIndex >= _sentences.Length)
            return "";
        return _sentences[_currentIndex];
    }

    public string GetSpeakerName() => _speakerName ?? "";

    /// <summary>대사만 바꿀 때 (연결층에서 퀘스트 등 사용).</summary>
    public void ReplaceContent(string speakerName, string[] sentences)
    {
        if (!IsTalking) return;
        _speakerName = speakerName ?? "";
        _sentences = sentences != null && sentences.Length > 0 ? sentences : Array.Empty<string>();
        _currentIndex = 0;
        OnDialogueStateChanged?.Invoke();
    }
}
