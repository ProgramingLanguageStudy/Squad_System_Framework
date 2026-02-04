[System.Serializable]
public class VisitTask : QuestTask
{
    public string TargetLocationId; // 방문해야 할 장소의 ID (씬 이름 등)

    // 방문은 도착 여부만 따지므로 TargetAmount는 기본적으로 1로 둡니다.
    public VisitTask()
    {
        TargetAmount = 1;
    }

    public override void UpdateProgress(string id, int amount)
    {
        // 전달된 장소 ID가 일치하면
        if (id == TargetLocationId)
        {
            // 방문 완료 (1/1)
            CurrentAmount = 1;
        }
    }
}
