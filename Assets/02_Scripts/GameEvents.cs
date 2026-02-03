public static class GameEvents
{
    // 어떤 ID(아이템, 몬스터, 장소)를 얼마만큼 진행했는지 알림
    public static System.Action<string, int> OnQuestGoalProcessed;

    // 퀘스트 상태가 바뀔 때 알림 (UI 갱신용)
    public static System.Action<QuestData> OnQuestUpdated;
}
