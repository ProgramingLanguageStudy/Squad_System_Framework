using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public string QuestId;
    public string Title;

    // 핵심: 여러 개의 목표를 가질 수 있음!
    [SerializeReference] // 다형성(상속) 데이터 저장을 위해 필요
    public List<QuestTask> Tasks = new List<QuestTask>();

    public bool IsAllTasksCompleted()
    {
        return Tasks.All(t => t.IsCompleted);
    }
}