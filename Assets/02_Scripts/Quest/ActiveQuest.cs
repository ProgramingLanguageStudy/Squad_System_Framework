using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>진행 중인 퀘스트 하나의 런타임 상태. QuestData(정의)를 복사해 생성하며, 에셋은 수정하지 않음.</summary>
public class ActiveQuest
{
    public string QuestId { get; }
    public string Title { get; }
    public IReadOnlyList<ActiveQuestTask> Tasks => _tasks;
    private readonly List<ActiveQuestTask> _tasks = new List<ActiveQuestTask>();

    public ActiveQuest(string questId, string title, List<ActiveQuestTask> tasks)
    {
        QuestId = questId;
        Title = title;
        if (tasks != null)
            _tasks.AddRange(tasks);
    }

    public bool IsAllTasksCompleted() => _tasks.All(t => t.IsCompleted);

    /// <summary>QuestData(에셋)에서 런타임용 ActiveQuest 생성. 에셋은 읽기만 하고 변경하지 않음.</summary>
    public static ActiveQuest CreateFrom(QuestData data)
    {
        if (data == null) return null;
        var tasks = new List<ActiveQuestTask>();
        foreach (var t in data.Tasks)
        {
            var at = new ActiveQuestTask
            {
                Description = t.Description,
                TargetAmount = t.TargetAmount,
                CurrentAmount = 0
            };
            if (t is GatherTask gt)
            {
                at.TargetId = gt.TargetItemId;
                at.IsAccumulate = false;
                at.RequiresItemDeduction = true;
            }
            else if (t is KillTask kt)
            {
                at.TargetId = kt.TargetMonsterId;
                at.IsAccumulate = true;
                at.RequiresItemDeduction = false;
            }
            else if (t is VisitTask vt)
            {
                at.TargetId = vt.TargetLocationId;
                at.IsAccumulate = false;
                at.RequiresItemDeduction = false;
            }
            tasks.Add(at);
        }
        return new ActiveQuest(data.QuestId, data.Title, tasks);
    }
}
