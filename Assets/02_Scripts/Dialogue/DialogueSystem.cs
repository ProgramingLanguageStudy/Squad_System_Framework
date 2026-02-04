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
    private bool _questButtonIsSubmit; // true면 제출, false면 수락

    /// <summary>지금 대화 중인 NPC ID. 퀘스트 버튼 클릭 시 사용.</summary>
    public string CurrentNpcId => _currentNpcId;
    /// <summary>첫 만남이면 퀘스트 버튼 숨김용.</summary>
    public DialogueType CurrentDialogueType => _currentDialogueType;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 대화 시작. 일상 대사일 때 퀘스트 수락/제출 버튼을 표시할 수 있습니다.
    /// </summary>
    public void StartDialogue(string speakerName, string[] sentences, string npcId, DialogueType dialogueType,
        bool showQuestButton, string questButtonText, Action onComplete = null,
        bool showSubmitButton = false, string submitButtonText = "제출하기")
    {
        if (IsTalking) return;
        IsTalking = true;
        _onDialogueComplete = onComplete;
        _currentSpeakerName = speakerName;
        _currentNpcId = npcId;
        _currentDialogueType = dialogueType;
        _questButtonIsSubmit = showSubmitButton;
        _ui.Open(speakerName, sentences, showQuestButton, questButtonText, showSubmitButton, submitButtonText);
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

    /// <summary>퀘스트 패널 버튼 클릭 시: 수락/제출 모드에 따라 분기.</summary>
    public void OnQuestPanelButtonClicked()
    {
        if (_questButtonIsSubmit)
            RequestQuestComplete();
        else
            RequestQuestDialogue();
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

    /// <summary>퀘스트 제출 버튼 클릭 시: 아이템 차감·퀘스트 완료 후 완료 대사를 표시.</summary>
    public void RequestQuestComplete()
    {
        if (string.IsNullOrEmpty(_currentNpcId)) return;
        if (!DialogueManager.Instance.GetCompletableQuestForNpc(_currentNpcId, out var quest, out var completionDialogue, out _))
            return;

        if (QuestManager.Instance == null || !QuestManager.Instance.CompleteQuest(quest.QuestId))
            return;

        string[] sentences = completionDialogue.Sentence.Split('/').Select(s => s.Trim()).ToArray();
        _ui.ReplaceContent(_currentSpeakerName, sentences);
        _ui.SetQuestButtonVisible(false);
    }
}
