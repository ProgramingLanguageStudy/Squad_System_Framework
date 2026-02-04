using UnityEngine;

/// <summary>
/// 대화·퀘스트·플래그가 섞인 쿼리. DialogueManager는 퀘스트를 모르므로 조율/코디네이터에서 사용.
/// </summary>
public static class QuestDialogueQueries
{
    /// <summary>이 NPC에게 완료 가능한 퀘스트가 있는지. (퀘스트 데이터 + 대화 데이터 조합)</summary>
    public static bool GetCompletableQuestForNpc(
        DialogueManager dialogueManager,
        IActiveQuestProvider activeQuestProvider,
        string npcId,
        out ActiveQuest quest,
        out DialogueData completionDialogue,
        out string completeButtonText)
    {
        quest = null;
        completionDialogue = null;
        completeButtonText = null;

        var questDialogue = dialogueManager.GetQuestDialogue(npcId);
        if (questDialogue == null || string.IsNullOrEmpty(questDialogue.LinkedQuestId)) return false;
        if (activeQuestProvider == null) return false;

        var activeQuests = activeQuestProvider.GetActiveQuests();
        ActiveQuest found = null;
        for (int i = 0; i < activeQuests.Count; i++)
        {
            var q = activeQuests[i];
            if (q.QuestId == questDialogue.LinkedQuestId && q.IsAllTasksCompleted()) { found = q; break; }
        }
        if (found == null) return false;

        completionDialogue = dialogueManager.GetQuestCompleteDialogue(npcId, found.QuestId);
        if (completionDialogue == null) return false;

        quest = found;
        completeButtonText = string.IsNullOrEmpty(completionDialogue.QuestButtonText) ? "완료" : completionDialogue.QuestButtonText;
        return true;
    }

    /// <summary>일상 대사 시 퀘스트 버튼 표시 가능 여부. (첫 대화 완료 + 수락 전 퀘스트 있을 때)</summary>
    public static bool GetAvailableQuestForNpc(
        DialogueManager dialogueManager,
        FlagManager flagManager,
        string npcId,
        out string questButtonText)
    {
        questButtonText = null;
        if (flagManager == null) return false;
        if (flagManager.GetFlag(GameStateKeys.FirstTalkNpc(npcId)) == 0) return false;

        var questDialogue = dialogueManager.GetQuestDialogue(npcId);
        if (questDialogue == null || string.IsNullOrEmpty(questDialogue.LinkedQuestId)) return false;
        if (flagManager.GetFlag(GameStateKeys.QuestAccepted(questDialogue.LinkedQuestId)) == 1) return false;

        questButtonText = string.IsNullOrEmpty(questDialogue.QuestButtonText) ? "퀘스트" : questDialogue.QuestButtonText;
        return true;
    }
}
