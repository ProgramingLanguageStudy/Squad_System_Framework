using System;

/// <summary>
/// 대화 상태만 보관. 현재 문장 반환·인덱스 노출. 제어(다음/종료)는 System에서.
/// </summary>
public class DialogueModel
{
    private DialogueData _data;
    private int _currentIndex;

    public bool IsTalking => _data != null;
    public int CurrentIndex => _currentIndex;
    public int LineCount => _data?.Lines?.Length ?? 0;

    public string CurrentNpcId => _data?.npcId ?? "";
    public string CurrentSpeakerName => _data?.npcId ?? "";
    public DialogueType CurrentDialogueType => _data != null ? _data.dialogueType : DialogueType.Common;
    /// <summary>Quest/QuestComplete일 때 퀘스트 ID. 버튼(수락/완료) 연동용.</summary>
    public string CurrentQuestId => _data != null ? _data.questId : null;

    public event Action OnDialogueStateChanged;
    public event Action OnDialogueEnd;

    /// <summary>System이 대화 시작 시 호출. 데이터만 넣고 인덱스 0.</summary>
    public void SetDialogue(DialogueData data)
    {
        _data = data;
        _currentIndex = 0;
        OnDialogueStateChanged?.Invoke();
    }

    /// <summary>System이 다음 문장으로 넘길 때 호출.</summary>
    public void SetCurrentIndex(int index)
    {
        _currentIndex = index;
        OnDialogueStateChanged?.Invoke();
    }

    /// <summary>System이 대화 종료 시 호출.</summary>
    public void Clear()
    {
        if (_data == null) return;
        _data = null;
        _currentIndex = 0;
        OnDialogueEnd?.Invoke();
        OnDialogueStateChanged?.Invoke();
    }

    public string GetCurrentSentence()
    {
        var lines = _data?.Lines;
        if (lines == null || _currentIndex < 0 || _currentIndex >= lines.Length)
            return "";
        return lines[_currentIndex];
    }

    public string GetSpeakerName() => CurrentSpeakerName;
}
