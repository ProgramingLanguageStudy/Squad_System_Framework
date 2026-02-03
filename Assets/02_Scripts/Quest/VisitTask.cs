[System.Serializable]
public class VisitTask : QuestTask
{
    public string TargetLocationId; // �����ؾ� �� ����� ID (�±׳� �̸�)

    // �湮�� ���� �� ���� �ϸ� �ǹǷ� TargetAmount�� �⺻������ 1�� �����մϴ�.
    public VisitTask()
    {
        TargetAmount = 1;
    }

    public override void UpdateProgress(string id, int amount)
    {
        // ������ ����� ID�� ��ġ�Ѵٸ�
        if (id == TargetLocationId)
        {
            // �湮 ���� (1/1)
            CurrentAmount = 1;
        }
    }
}