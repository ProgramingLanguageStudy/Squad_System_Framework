using System;

/// <summary>퀘스트 목표 하나의 런타임 진행 상태. QuestData의 Task 정의를 복사해 생성.</summary>
[Serializable]
public class ActiveQuestTask
{
    public string Description;
    public int TargetAmount;
    public int CurrentAmount;
    /// <summary>진행 신호와 매칭할 ID (Gather=ItemId, Kill=MonsterId, Visit=LocationId).</summary>
    public string TargetId;
    /// <summary>true면 CurrentAmount += amount (Kill), false면 CurrentAmount = amount (Gather/Visit).</summary>
    public bool IsAccumulate;
    /// <summary>완료 시 인벤토리에서 차감할지 (Gather만 true).</summary>
    public bool RequiresItemDeduction;

    public bool IsCompleted => CurrentAmount >= TargetAmount;

    public void UpdateProgress(string id, int amount)
    {
        if (id != TargetId) return;
        if (IsAccumulate)
            CurrentAmount = Math.Min(CurrentAmount + amount, TargetAmount);
        else
            CurrentAmount = amount;
    }
}
