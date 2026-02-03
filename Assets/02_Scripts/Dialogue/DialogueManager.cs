using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : Singleton<DialogueManager>
{
    // CSV에서 변환된 모든 DialogueData 에셋들을 담아두는 리스트
    private List<DialogueData> _dialogueDatabase = new List<DialogueData>();

    // 로드 완료 여부를 확인하기 위한 프로퍼티
    public bool IsLoaded { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake(); // 싱글톤 초기화 실행
        LoadAllDialogueAssets();
    }

    /// <summary>
    /// Resources/Dialogues 폴더 내의 모든 DialogueData 에셋을 메모리로 로드합니다.
    /// </summary>
    private void LoadAllDialogueAssets()
    {
        // 툴로 생성했던 .asset 파일들을 싹 긁어옵니다.
        DialogueData[] assets = Resources.LoadAll<DialogueData>("Dialogues");

        if (assets == null || assets.Length == 0)
        {
            Debug.LogWarning("<color=yellow>[DialogueManager]</color> 로드할 대사 에셋이 없습니다. CSV Importer를 확인하세요.");
            return;
        }

        _dialogueDatabase = assets.ToList();
        IsLoaded = true;

        Debug.Log($"<color=cyan>[DialogueManager]</color> 총 {_dialogueDatabase.Count}개의 대사 데이터를 로드했습니다.");
    }

    /// <summary>
    /// NPC ID를 기반으로 현재 상황(퀘스트, 호감도 등)에 가장 적합한 대사 한 줄을 반환합니다.
    /// </summary>
    public DialogueData GetBestDialogue(string npcId)
    {
        // 1. 해당 NPC의 대사만 필터링
        var npcTalks = _dialogueDatabase.Where(d => d.NpcId == npcId).ToList();

        if (npcTalks.Count == 0)
        {
            Debug.LogError($"[DialogueManager] {npcId}에 해당하는 대사 데이터를 찾을 수 없습니다.");
            return null;
        }

        // 2. 우선순위 판별 (지금은 단순 예시지만, 나중에 여기에 퀘스트 단계 체크 로직이 들어갑니다)

        // 2-1. 퀘스트 대사 (ConditionKey와 Value가 현재 퀘스트 시스템과 일치하는지 확인)
        // DialogueData questTalk = npcTalks.FirstOrDefault(t => t.DialogueType == DialogueType.Quest && CheckQuestCondition(t));
        // if (questTalk != null) return questTalk;

        // 2-2. 호감도 대사
        // DialogueData affectionTalk = npcTalks.FirstOrDefault(t => t.DialogueType == DialogueType.Affection && CheckAffection(t));
        // if (affectionTalk != null) return affectionTalk;

        // 2-3. 기본 대사 (Common)
        // 여러 개일 경우 그중 랜덤으로 하나를 반환하게 하면 게임이 더 자연스럽습니다.
        var commonTalks = npcTalks.Where(t => t.DialogueType == DialogueType.Common).ToList();
        if (commonTalks.Count > 0)
        {
            int randomIndex = Random.Range(0, commonTalks.Count);
            return commonTalks[randomIndex];
        }

        return npcTalks[0]; // 최후의 보루: 리스트의 첫 번째 데이터라도 반환
    }
}