using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Data")]
public class DialogueData : ScriptableObject
{
    public string NpcName; // 대본용 이름 (비워두면 NPC 이름 사용)
    public string QuestKey;

    [Header("--- 퀘스트용 브랜치 ---")]
    public List<QuestDialogueBranch> QuestBranches;

    [Header("--- 일상용 대사 ---")]
    public List<DialogueGroup> NormalGroups;
}

[System.Serializable]
public struct QuestDialogueBranch
{
    public QuestState RequiredState;
    [TextArea(3, 5)] public string[] Sentences;
    public int NextStepValue; // 대화 후 변경될 단계 (없으면 -1)
}

[System.Serializable]
public struct DialogueGroup
{
    [TextArea(3, 5)] public string[] Sentences;
}