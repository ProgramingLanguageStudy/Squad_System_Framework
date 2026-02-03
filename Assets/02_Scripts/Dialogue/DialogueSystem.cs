using System;
using UnityEngine;
using System.Linq;

public class DialogueSystem : Singleton<DialogueSystem>
{
    [SerializeField] private DialogueUI _ui;
    public bool IsTalking { get; private set; }
    public event Action OnDialogueEnd;

    private Action _onDialogueComplete;
    private string _currentSpeakerName;
    private string _currentNpcId;
    private DialogueType _currentDialogueType;

    /// <summary>지금 대화 중인 NPC ID. 퀘스트 버튼 클릭 시 사용.</summary>
    public string CurrentNpcId => _currentNpcId;
    /// <summary>첫 만남이면 퀘스트 버튼 숨김용.</summary>
    public DialogueType CurrentDialogueType => _currentDialogueType;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 대화 시작. 일상 대사일 때만 showQuestButton=true로 호출합니다.
    /// </summary>
    public void StartDialogue(string speakerName, string[] sentences, string npcId, DialogueType dialogueType, bool showQuestButton, string questButtonText, Action onComplete = null)
    {
        if (IsTalking) return;
        IsTalking = true;
        _onDialogueComplete = onComplete;
        _currentSpeakerName = speakerName;
        _currentNpcId = npcId;
        _currentDialogueType = dialogueType;
        _ui.Open(speakerName, sentences, showQuestButton, questButtonText);
    }

    /// <summary>다음 문장으로. 끝이 나도 자동 종료하지 않음 (대화 끝내기 버튼으로만 종료).</summary>
    public void DisplayNextSentence()
    {
        if (!IsTalking) return;
        if (_ui.ShowNext())
        {
            // 문장은 끝났지만 창은 유지. 버튼으로만 닫기.
        }
    }

    /// <summary>대화창 닫기. 버튼에서 호출.</summary>
    public void EndDialogue()
    {
        if (!IsTalking) return;
        IsTalking = false;
        _ui.Close();
        OnDialogueEnd?.Invoke();
        _onDialogueComplete?.Invoke();
        _onDialogueComplete = null;
        _currentNpcId = null;
        _currentDialogueType = DialogueType.Common;
    }

    /// <summary>퀘스트 버튼 클릭 시: 퀘스트 제시 대사로 바꾸고, 끝나면 수락 처리.</summary>
    public void RequestQuestDialogue()
    {
        if (string.IsNullOrEmpty(_currentNpcId)) return;
        var questDialogue = DialogueManager.Instance.GetQuestDialogue(_currentNpcId);
        if (questDialogue == null) return;

        string[] sentences = questDialogue.Sentence.Split('/').Select(s => s.Trim()).ToArray();
        Action questOnComplete = () =>
        {
            var flagManager = FindFirstObjectByType<FlagManager>();
            if (flagManager != null && !string.IsNullOrEmpty(questDialogue.LinkedQuestId))
            {
                flagManager.SetFlag(GameStateKeys.QuestAccepted(questDialogue.LinkedQuestId), 1);
                var questData = Resources.Load<QuestData>($"Quests/{questDialogue.LinkedQuestId}");
                if (questData != null && QuestManager.Instance != null)
                    QuestManager.Instance.AcceptQuest(questData);
            }
        };

        _onDialogueComplete = questOnComplete;
        _currentDialogueType = DialogueType.Quest;
        _ui.ReplaceContent(_currentSpeakerName, sentences);
        _ui.SetQuestButtonVisible(false);
    }
}
