using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : Singleton<QuestManager>
{
    // 현재 수락해서 진행 중인 퀘스트 목록
    [SerializeField] private List<QuestData> _activeQuests = new List<QuestData>();

    // 이미 완료된 퀘스트 ID 저장 (중복 수락 방지 및 세이브용)
    private HashSet<string> _completedQuestIds = new HashSet<string>();

    private void OnEnable()
    {
        // 이벤트 구독: 무언가 목표와 관련된 행동이 일어나면 실행
        GameEvents.OnQuestGoalProcessed += ProcessQuestProgress;
    }

    private void OnDisable()
    {
        GameEvents.OnQuestGoalProcessed -= ProcessQuestProgress;
    }

    /// <summary>
    /// 외부(인벤토리, 전투, 트리거)에서 신호를 보낼 때 호출됩니다.
    /// </summary>
    private void ProcessQuestProgress(string id, int amount)
    {
        foreach (var quest in _activeQuests)
        {
            // 퀘스트 내의 모든 Task에게 소식을 알림
            foreach (var task in quest.Tasks)
            {
                if (!task.IsCompleted)
                {
                    task.UpdateProgress(id, amount);
                }
            }

            // 모든 Task가 끝났는지 체크
            if (quest.IsAllTasksCompleted())
            {
                Debug.Log($"<color=green>{quest.Title}</color> 목표 달성! NPC에게 돌아가세요.");
            }

            // UI 등에 변화를 알림
            GameEvents.OnQuestUpdated?.Invoke(quest);
        }
    }

    // 퀘스트 수락
    public void AcceptQuest(QuestData quest)
    {
        _activeQuests.Add(quest);

        // 퀘스트를 받자마자 이미 인벤토리에 있는 아이템들을 체크 (소급 적용)
        foreach (var task in quest.Tasks)
        {
            if (task is GatherTask gatherTask)
            {
                // 인벤토리에 현재 몇 개 있는지 물어보고 업데이트
                int currentAmount = InventoryManager.Instance.GetTotalCount(gatherTask.TargetItemId);
                gatherTask.UpdateProgress(gatherTask.TargetItemId, currentAmount);
            }
        }

        GameEvents.OnQuestUpdated?.Invoke(quest);
    }
}