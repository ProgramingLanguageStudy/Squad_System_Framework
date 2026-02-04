using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuestManager : Singleton<QuestManager>
{
    [SerializeField] private List<QuestData> _activeQuests = new List<QuestData>();

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

    /// <summary>진행 중인 퀘스트 목록 (퀘스트 UI 등에서 사용).</summary>
    public IReadOnlyList<QuestData> GetActiveQuests() => _activeQuests;

    /// <summary>
    /// 퀘스트 제출: 수집 아이템을 인벤토리에서 차감하고, 퀘스트를 완료 처리합니다.
    /// </summary>
    /// <returns>성공 시 true (아이템 부족 등이면 false)</returns>
    public bool CompleteQuest(string questId)
    {
        var quest = _activeQuests.FirstOrDefault(q => q.QuestId == questId);
        if (quest == null) return false;

        // 수집 퀘스트: 인벤토리에서 목표 수량만큼 제거
        foreach (var task in quest.Tasks)
        {
            if (task is GatherTask gatherTask)
            {
                if (InventoryManager.Instance == null) return false;
                if (!InventoryManager.Instance.RemoveItem(gatherTask.TargetItemId, gatherTask.TargetAmount))
                    return false;
            }
        }

        _activeQuests.Remove(quest);
        var flagManager = FindFirstObjectByType<FlagManager>();
        if (flagManager != null)
            flagManager.SetFlag(GameStateKeys.QuestCompleted(questId), 1);

        GameEvents.OnQuestUpdated?.Invoke(quest);
        return true;
    }

    // --- 대화창 퀘스트 버튼 처리 (PlayScene이 DialogueSystem 이벤트로 연결) ---

    /// <summary>대화창에서 퀘스트 수락 버튼 클릭 시. 퀘스트 관련 처리는 이 클래스 한 곳에서.</summary>
    public void HandleQuestAcceptRequestedFromDialogue(string npcId)
    {
        var questDialogue = DialogueManager.Instance.GetQuestDialogue(npcId);
        if (questDialogue == null || string.IsNullOrEmpty(questDialogue.LinkedQuestId)) return;

        string[] sentences = questDialogue.Sentence.Split('/').Select(s => s.Trim()).ToArray();
        var ds = DialogueSystem.Instance;
        ds.ReplaceContent(ds.CurrentSpeakerName, sentences);
        ds.SetQuestButtonVisible(false);

        ds.RegisterOnDialogueEndOnce(() =>
        {
            var flagManager = FindFirstObjectByType<FlagManager>();
            if (flagManager != null)
                flagManager.SetFlag(GameStateKeys.QuestAccepted(questDialogue.LinkedQuestId), 1);
            var questData = Resources.Load<QuestData>($"Quests/{questDialogue.LinkedQuestId}");
            if (questData != null)
                AcceptQuest(questData);
        });
    }

    /// <summary>대화창에서 퀘스트 제출 버튼 클릭 시.</summary>
    public void HandleQuestSubmitRequestedFromDialogue(string npcId)
    {
        if (!DialogueManager.Instance.GetCompletableQuestForNpc(npcId, out var quest, out var completionDialogue, out _))
            return;
        if (!CompleteQuest(quest.QuestId)) return;

        string[] sentences = completionDialogue.Sentence.Split('/').Select(s => s.Trim()).ToArray();
        var ds = DialogueSystem.Instance;
        ds.ReplaceContent(ds.CurrentSpeakerName, sentences);
        ds.SetQuestButtonVisible(false);
    }
}