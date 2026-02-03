[System.Serializable]
public class GatherTask : QuestTask
{
    public string TargetItemId; // �����ؾ� �� ������ ID

    public override void UpdateProgress(string id, int amount)
    {
        // ���� ��ٸ��� ������ ID�� �´ٸ� ��ġ ����!
        if (id == TargetItemId)
        {
            CurrentAmount = amount;
        }
    }
}