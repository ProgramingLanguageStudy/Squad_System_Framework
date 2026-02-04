[System.Serializable]
public class KillTask : QuestTask
{
    public string TargetMonsterId; // óġ�ؾ� �� ���� ID

    public override void UpdateProgress(string id, int amount)
    {
        // ���� ���� ID�� ���� ��ǥ�� �� ���̶��
        if (id == TargetMonsterId)
        {
            // óġ�� ���� ����(+=) ����Դϴ�.
            CurrentAmount += amount;

            // ��ǥġ�� ���� �ʵ��� ���� (���� ����)
            if (CurrentAmount > TargetAmount) CurrentAmount = TargetAmount;
        }
    }
}