using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// 대화 시작을 조율. DialogueManager·Quest·Flag를 알고 있지만, 각 시스템은 서로를 모릅니다.
/// Npc가 이 코디네이터에만 의존하도록 합니다.
/// </summary>
public class DialogueCoordinator : MonoBehaviour
{
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private DialogueSystem _dialogueSystem;
    [SerializeField] private FlagManager _flagManager;
    private IActiveQuestProvider _questProvider;

    /// <summary>PlayScene 등에서 한 번 호출.</summary>
    public void Initialize(DialogueManager dm, DialogueSystem ds, IActiveQuestProvider questProvider, FlagManager flagManager)
    {
        _dialogueManager = dm;
        _dialogueSystem = ds;
        _questProvider = questProvider;
        _flagManager = flagManager;
    }

    /// <summary>NPC와 대화 시작. 퀘스트/완료 버튼 여부는 여기서 판단.</summary>
    public void StartDialogue(string npcId)
    {
        var data = _dialogueManager != null ? _dialogueManager.GetBestDialogue(npcId) : null;
        if (data == null) return;

        string[] sentences = data.Sentence.Split('/').Select(s => s.Trim()).ToArray();
        string questButtonText = "퀘스트";
        bool showQuestButton = false;
        bool showCompleteButton = false;
        string completeButtonText = "완료";

        if (data.DialogueType == DialogueType.Common || data.DialogueType == DialogueType.Affection)
        {
            if (_questProvider != null && _dialogueManager != null &&
                QuestDialogueQueries.GetCompletableQuestForNpc(_dialogueManager, _questProvider, npcId, out _, out _, out completeButtonText))
                showCompleteButton = true;
            else if (_flagManager != null && _dialogueManager != null &&
                QuestDialogueQueries.GetAvailableQuestForNpc(_dialogueManager, _flagManager, npcId, out questButtonText))
                showQuestButton = true;
        }

        Action onDialogueEnd = null;
        if (_flagManager != null)
            onDialogueEnd = () => _flagManager.SetFlag(GameStateKeys.FirstTalkNpc(npcId), 1);

        if (_dialogueSystem != null)
            _dialogueSystem.StartDialogue(npcId, sentences, npcId, data.DialogueType,
                showQuestButton, questButtonText ?? "퀘스트", onDialogueEnd, showCompleteButton, completeButtonText);
    }
}
