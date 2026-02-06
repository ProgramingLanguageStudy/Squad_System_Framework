using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 퀘스트 시스템. 다른 시스템을 알지 않음. 조율층이 NotifyProgress(targetId) 호출 시 해당 targetId 퀘스트의 current를 1 올림.
/// </summary>
public class QuestSystem : MonoBehaviour
{
    private readonly List<QuestModel> _activeQuests = new List<QuestModel>();

    public event Action<QuestModel> OnQuestUpdated;

    /// <summary>targetId에 해당하는 진행이 일어났을 때 호출. 해당 퀘스트 current를 1 올림 (방문/채집/처치 공통).</summary>
    public void NotifyProgress(string targetId)
    {
        foreach (var quest in _activeQuests)
        {
            if (quest.IsCompleted || quest.TargetId != targetId) continue;
            quest.CurrentAmount = Math.Min(quest.CurrentAmount + 1, quest.TargetAmount);
            if (quest.IsCompleted)
                Debug.Log($"<color=green>{quest.Title}</color> 목표 달성! NPC에게 돌아가세요.");
            OnQuestUpdated?.Invoke(quest);
        }
    }

    /// <summary>수락 시 등, 현재 진행도를 한 번에 설정할 때 (예: 인벤토리 개수 동기화).</summary>
    public void SetTaskProgress(string questId, string targetId, int amount)
    {
        var quest = _activeQuests.FirstOrDefault(q => q.QuestId == questId && q.TargetId == targetId);
        if (quest == null) return;
        quest.CurrentAmount = Math.Min(amount, quest.TargetAmount);
        OnQuestUpdated?.Invoke(quest);
    }

    public void AcceptQuest(QuestData questData)
    {
        var quest = new QuestModel(questData);
        _activeQuests.Add(quest);
        OnQuestUpdated?.Invoke(quest);
    }

    /// <summary>퀘스트 완료: 목록에서만 제거. 아이템 차감·플래그는 조율에서 처리 후 호출.</summary>
    public bool CompleteQuest(string questId)
    {
        var quest = _activeQuests.FirstOrDefault(q => q.QuestId == questId);
        if (quest == null) return false;
        _activeQuests.Remove(quest);
        OnQuestUpdated?.Invoke(quest);
        return true;
    }

    public IReadOnlyList<QuestModel> GetActiveQuests() => _activeQuests;
}
