using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : Singleton<DialogueManager>
{
    private List<DialogueData> _dialogueDatabase = new List<DialogueData>();
    /// <summary>NpcId별 대화 목록 (로드 시 1회만 구성, GetBestDialogue 등에서 GC 없이 조회)</summary>
    private Dictionary<string, List<DialogueData>> _dialogueByNpcId = new Dictionary<string, List<DialogueData>>();

    private Dictionary<string, int> _affectionTable = new Dictionary<string, int>();

    private FlagManager _cachedFlagManager;

    // 로드 완료 여부를 확인하기 위한 프로퍼티
    public bool IsLoaded { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake();
        // Resources.LoadAll은 첫 프레임에 부담을 줌 → 다음 프레임으로 미룸
        StartCoroutine(LoadAllDialogueAssetsNextFrame());
    }

    private FlagManager GetCachedFlagManager()
    {
        if (_cachedFlagManager == null)
            _cachedFlagManager = FindFirstObjectByType<FlagManager>();
        return _cachedFlagManager;
    }

    /// <summary>
    /// 다음 프레임에 대화 데이터 로드. 첫 Play 시 렉 방지.
    /// </summary>
    private IEnumerator LoadAllDialogueAssetsNextFrame()
    {
        yield return null;
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

        _dialogueDatabase = new List<DialogueData>(assets);
        _dialogueByNpcId.Clear();
        for (int i = 0; i < assets.Length; i++)
        {
            var d = assets[i];
            if (string.IsNullOrEmpty(d.NpcId)) continue;
            if (!_dialogueByNpcId.TryGetValue(d.NpcId, out var list))
            {
                list = new List<DialogueData>();
                _dialogueByNpcId[d.NpcId] = list;
            }
            list.Add(d);
        }
        IsLoaded = true;

        Debug.Log($"<color=cyan>[DialogueManager]</color> 총 {_dialogueDatabase.Count}개의 대화 데이터를 로드했습니다.");
    }

    /// <summary>
    /// NPC ID에 맞춰 현재 상태를 고려해서 가장 적합한 대화 데이터를 반환합니다.
    /// </summary>
    public DialogueData GetBestDialogue(string npcId)
    {
        if (!IsLoaded || !_dialogueByNpcId.TryGetValue(npcId, out var npcTalks) || npcTalks.Count == 0)
        {
            if (IsLoaded) Debug.LogError($"[DialogueManager] {npcId}에 해당하는 대화 데이터가 없습니다.");
            return null;
        }

        int currentAffection = GetAffection(npcId);
        var flagManager = GetCachedFlagManager();

        // 3-0. 첫 만남
        if (flagManager != null && flagManager.GetFlag(GameStateKeys.FirstTalkNpc(npcId)) == 0)
        {
            for (int i = 0; i < npcTalks.Count; i++)
            {
                if (npcTalks[i].DialogueType == DialogueType.FirstMeet)
                    return npcTalks[i];
            }
        }

        // 3-2. 호감도 대사 (조건 만족 중 호감도 최고)
        DialogueData bestAffection = null;
        for (int i = 0; i < npcTalks.Count; i++)
        {
            var t = npcTalks[i];
            if (t.DialogueType != DialogueType.Affection || t.ConditionValue > currentAffection) continue;
            if (bestAffection == null || t.ConditionValue > bestAffection.ConditionValue)
                bestAffection = t;
        }
        if (bestAffection != null) return bestAffection;

        // 3-3. Common 중 랜덤
        int commonCount = 0;
        for (int i = 0; i < npcTalks.Count; i++)
            if (npcTalks[i].DialogueType == DialogueType.Common) commonCount++;
        if (commonCount > 0)
        {
            int pick = Random.Range(0, commonCount);
            for (int i = 0; i < npcTalks.Count; i++)
            {
                if (npcTalks[i].DialogueType != DialogueType.Common) continue;
                if (pick-- == 0) return npcTalks[i];
            }
        }

        return npcTalks[0];
    }

    /// <summary>
    /// 해당 NPC의 퀘스트 제시용 대사 1개를 반환합니다. (DialogueType.Quest)
    /// </summary>
    public DialogueData GetQuestDialogue(string npcId)
    {
        if (!_dialogueByNpcId.TryGetValue(npcId, out var list)) return null;
        for (int i = 0; i < list.Count; i++)
            if (list[i].DialogueType == DialogueType.Quest) return list[i];
        return null;
    }

    public DialogueData GetQuestCompleteDialogue(string npcId, string questId)
    {
        if (!_dialogueByNpcId.TryGetValue(npcId, out var list)) return null;
        for (int i = 0; i < list.Count; i++)
        {
            var d = list[i];
            if (d.DialogueType == DialogueType.QuestComplete && d.LinkedQuestId == questId)
                return d;
        }
        return null;
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
