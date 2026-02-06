/// <summary>진행 중인 퀘스트 하나의 런타임 상태. QuestData를 복사해 생성.</summary>
public class ActiveQuest
{
    public string QuestId { get; }
    public string Title { get; }
    public string Description { get; }
    public QuestType QuestType { get; }
    public string TargetId { get; }
    public int TargetAmount { get; }
    public int CurrentAmount { get; set; }
    public bool RequiresItemDeduction { get; }

    public bool IsCompleted => CurrentAmount >= TargetAmount;
    /// <summary>호환용. 단일 목표이므로 IsCompleted와 동일.</summary>
    public bool IsAllTasksCompleted() => IsCompleted;

    public ActiveQuest(string questId, string title, string description, QuestType questType, string targetId, int targetAmount, bool requiresItemDeduction)
    {
        QuestId = questId;
        Title = title;
        Description = description;
        QuestType = questType;
        TargetId = targetId;
        TargetAmount = targetAmount;
        CurrentAmount = 0;
        RequiresItemDeduction = requiresItemDeduction;
    }

    public static ActiveQuest CreateFrom(QuestData data)
    {
        if (data == null) return null;
        return new ActiveQuest(
            data.QuestId,
            data.Title,
            data.Description,
            data.QuestType,
            data.TargetId,
            data.TargetAmount,
            data.RequiresItemDeduction
        );
    }
}
