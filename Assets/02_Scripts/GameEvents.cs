using System;

public static class GameEvents
{
    // 어떤 ID(아이템, 몬스터, 장소)를 얼마만큼 진행했는지 알림 (QuestModel이 구독)
    public static Action<string, int> OnQuestGoalProcessed;
}
