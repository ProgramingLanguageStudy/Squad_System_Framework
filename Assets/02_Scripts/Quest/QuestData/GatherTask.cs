[System.Serializable]
public class GatherTask : QuestTask
{
    public string TargetItemId; // 수집해야 할 아이템 ID

    public override void UpdateProgress(string id, int amount)
    {
        // 전달된 아이템 ID가 일치하면 진행도 반영
        if (id == TargetItemId)
        {
            CurrentAmount = amount;
        }
    }
}
