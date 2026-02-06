using System;

public static class GameEvents
{
    /// <summary>어떤 ID(아이템, 몬스터, 장소)를 얼마만큼 진행했는지 알림 (조율층에서 QuestSystem.NotifyProgress 호출용)</summary>
    public static Action<string, int> OnQuestGoalProcessed;

    /// <summary>대화 재생 요청. DialogueSystem이 구독해 재생. 연결 시 DialogueInteractor 등에서 호출.</summary>
    public static Action<DialogueData> OnPlayDialogueRequested;
}
