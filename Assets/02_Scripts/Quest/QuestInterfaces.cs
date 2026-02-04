using System.Collections.Generic;

/// <summary>진행 중인 퀘스트 목록 조회. QuestDialogueQueries·DialogueCoordinator 등에서 사용합니다.</summary>
public interface IActiveQuestProvider
{
    IReadOnlyList<ActiveQuest> GetActiveQuests();
}
