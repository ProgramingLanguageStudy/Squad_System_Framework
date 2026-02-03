public static class GameEvents
{
    // � ID(������, ����, ���)�� � ������ŭ ��ȭ�ߴ��� �˸�
    public static System.Action<string, int> OnQuestGoalProcessed;

    // ����Ʈ ���°� ������ �� �˸� (UI ���ſ�)
    public static System.Action<QuestData> OnQuestUpdated;
}