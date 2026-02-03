using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : Singleton<DialogueManager>
{
    // CSV로 변환된 모든 DialogueData 에셋들을 리스트로 보관
    private List<DialogueData> _dialogueDatabase = new List<DialogueData>();

    // NPC ID별 호감도 값을 저장하는 테이블 (중요!)
    private Dictionary<string, int> _affectionTable = new Dictionary<string, int>();

    // 로드 완료 여부를 확인하기 위한 프로퍼티
    public bool IsLoaded { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake(); // 싱글톤 초기화 호출
        LoadAllDialogueAssets();
    }

    /// <summary>
    /// Resources/Dialogues 폴더에 있는 모든 DialogueData 에셋을 메모리에 로드합니다.
    /// </summary>
    private void LoadAllDialogueAssets()
    {
        DialogueData[] assets = Resources.LoadAll<DialogueData>("Dialogues");

        if (assets == null || assets.Length == 0)
        {
            Debug.LogWarning("<color=yellow>[DialogueManager]</color> 로드할 대화 데이터가 없습니다.");
            return;
        }

        _dialogueDatabase = assets.ToList();
        IsLoaded = true;

        Debug.Log($"<color=cyan>[DialogueManager]</color> 총 {_dialogueDatabase.Count}개의 대화 데이터를 로드했습니다.");
    }

    /// <summary>
    /// NPC ID에 맞춰 현재 상태를 고려해서 가장 적합한 대화 데이터를 반환합니다.
    /// </summary>
    public DialogueData GetBestDialogue(string npcId)
    {
        // 1. 해당 NPC의 모든 대화 데이터
        var npcTalks = _dialogueDatabase.Where(d => d.NpcId == npcId).ToList();

        if (npcTalks.Count == 0)
        {
            Debug.LogError($"[DialogueManager] {npcId}에 해당하는 대화 데이터가 없습니다.");
            return null;
        }

        // 2. 현재 NPC의 호감도 값 가져오기
        int currentAffection = GetAffection(npcId);

        // 3. 우선순위 체크

        // 3-0. 첫 만남: 아직 말한 적 없으면 FirstMeet 대사 사용
        var flagManager = FindFirstObjectByType<FlagManager>();
        if (flagManager != null && flagManager.GetFlag(GameStateKeys.FirstTalkNpc(npcId)) == 0)
        {
            var firstMeet = npcTalks.FirstOrDefault(t => t.DialogueType == DialogueType.FirstMeet);
            if (firstMeet != null) return firstMeet;
        }

        // 3-1. 퀘스트 대사 (나중에 QuestManager 연동 시 활성화)
        // DialogueData questTalk = npcTalks.FirstOrDefault(t => t.DialogueType == DialogueType.Quest && CheckQuest(t));
        // if (questTalk != null) return questTalk;

        // 3-2. 호감도 대사 (조건 만족하는 것 중 호감도 높은 순으로 선택)
        var affectionTalk = npcTalks
            .Where(t => t.DialogueType == DialogueType.Affection)
            .Where(t => t.ConditionValue <= currentAffection)
            .OrderByDescending(t => t.ConditionValue)
            .FirstOrDefault();

        if (affectionTalk != null) return affectionTalk;

        // 3-3. 기본 대사 (Common) 중 랜덤 반환
        var commonTalks = npcTalks.Where(t => t.DialogueType == DialogueType.Common).ToList();
        if (commonTalks.Count > 0)
        {
            return commonTalks[Random.Range(0, commonTalks.Count)];
        }

        return npcTalks[0];
    }

    /// <summary>
    /// 해당 NPC의 퀘스트 제시용 대사 1개를 반환합니다. (DialogueType.Quest)
    /// </summary>
    public DialogueData GetQuestDialogue(string npcId)
    {
        return _dialogueDatabase
            .FirstOrDefault(d => d.NpcId == npcId && d.DialogueType == DialogueType.Quest);
    }

    /// <summary>
    /// 일상 대사 시 퀘스트 버튼을 보여줄 수 있는지 확인합니다.
    /// 첫 만남 완료 + 수락 전 퀘스트가 있을 때만 true, 버튼 텍스트를 반환합니다.
    /// </summary>
    public bool GetAvailableQuestForNpc(string npcId, out string questButtonText)
    {
        questButtonText = null;
        var flagManager = FindFirstObjectByType<FlagManager>();
        if (flagManager == null) return false;
        if (flagManager.GetFlag(GameStateKeys.FirstTalkNpc(npcId)) == 0) return false;

        var questDialogue = GetQuestDialogue(npcId);
        if (questDialogue == null || string.IsNullOrEmpty(questDialogue.LinkedQuestId)) return false;
        if (flagManager.GetFlag(GameStateKeys.QuestAccepted(questDialogue.LinkedQuestId)) == 1) return false;

        questButtonText = string.IsNullOrEmpty(questDialogue.QuestButtonText) ? "퀘스트" : questDialogue.QuestButtonText;
        return true;
    }

    // --- 호감도 관련 API ---

    /// <summary>
    /// NPC의 현재 호감도 값을 반환합니다.
    /// </summary>
    public int GetAffection(string npcId)
    {
        if (_affectionTable.ContainsKey(npcId))
            return _affectionTable[npcId];
        return 0;
    }

    /// <summary>
    /// NPC의 호감도를 증가시킵니다.
    /// </summary>
    public void AddAffection(string npcId, int amount)
    {
        if (!_affectionTable.ContainsKey(npcId))
            _affectionTable[npcId] = 0;

        _affectionTable[npcId] += amount;
        Debug.Log($"<color=magenta>[Affection]</color> {npcId}의 현재 호감도: {_affectionTable[npcId]}");
    }
}
