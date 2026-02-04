using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 퀘스트 상태와 도메인 로직만 담당 (MVP의 Model).
/// QuestData(정의)는 수락 시 ActiveQuest(런타임)로 복사해 보관하며, 에셋은 수정하지 않습니다.
/// </summary>
public class QuestModel : MonoBehaviour, IActiveQuestProvider
{
    private readonly List<ActiveQuest> _activeQuests = new List<ActiveQuest>();

    /// <summary>진행 중인 퀘스트 목록이 바뀔 때 발행 (UI 갱신용).</summary>
    public event Action<ActiveQuest> OnQuestUpdated;

    private void OnEnable()
    {
        GameEvents.OnQuestGoalProcessed += ProcessQuestProgress;
    }

    private void OnDisable()
    {
        GameEvents.OnQuestGoalProcessed -= ProcessQuestProgress;
    }

    /// <summary>외부(인벤토리, 전투, 트리거)에서 목표 진행 신호를 보낼 때 호출됩니다.</summary>
    private void ProcessQuestProgress(string id, int amount)
    {
        foreach (var quest in _activeQuests)
        {
            foreach (var task in quest.Tasks)
            {
                if (!task.IsCompleted)
                    task.UpdateProgress(id, amount);
            }
            if (quest.IsAllTasksCompleted())
                Debug.Log($"<color=green>{quest.Title}</color> 목표 달성! NPC에게 돌아가세요.");
            OnQuestUpdated?.Invoke(quest);
        }
    }

    /// <summary>퀘스트 수락. QuestData에서 ActiveQuest를 생성해 보관. 수락 플래그·수집 소급 적용은 조율(PlayScene 등)에서 SetGatherProgress 등으로 처리합니다.</summary>
    public void AcceptQuest(QuestData questData)
    {
        var quest = ActiveQuest.CreateFrom(questData);
        if (quest == null) return;
        _activeQuests.Add(quest);
        OnQuestUpdated?.Invoke(quest);
    }

    /// <summary>수집 퀘스트의 진행도를 외부에서 설정. 수락 시 인벤토리 개수 동기화용.</summary>
    public void SetGatherProgress(string questId, string itemId, int amount)
    {
        var quest = _activeQuests.FirstOrDefault(q => q.QuestId == questId);
        if (quest == null) return;
        foreach (var task in quest.Tasks)
        {
            if (task.TargetId == itemId)
            {
                task.UpdateProgress(itemId, amount);
                OnQuestUpdated?.Invoke(quest);
                return;
            }
        }
    }

    /// <summary>퀘스트 완료: 목록에서만 제거. 아이템 차감·플래그 설정은 조율(PlayScene 등)에서 먼저 처리 후 호출합니다.</summary>
    /// <returns>해당 퀘스트가 있어 제거했으면 true</returns>
    public bool CompleteQuest(string questId)
    {
        var quest = _activeQuests.FirstOrDefault(q => q.QuestId == questId);
        if (quest == null) return false;
        _activeQuests.Remove(quest);
        OnQuestUpdated?.Invoke(quest);
        return true;
    }

    /// <summary>진행 중인 퀘스트 목록 (UI·다른 시스템 조회용).</summary>
    public IReadOnlyList<ActiveQuest> GetActiveQuests() => _activeQuests;
}
