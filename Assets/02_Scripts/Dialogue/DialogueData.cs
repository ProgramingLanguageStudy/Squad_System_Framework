using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Data")]
public class DialogueData : ScriptableObject
{
    public string NpcId;
    public DialogueType DialogueType;
    public string ConditionKey = string.Empty;
    public int ConditionValue;
    [TextArea(3, 5)] public string Sentence;

    public string AfterActionEvent = string.Empty;

    /// <summary>이 대사가 퀘스트 제시용일 때 연결된 퀘스트 ID (Resources/Quests/{id}.asset).</summary>
    public string LinkedQuestId = string.Empty;
    /// <summary>대화창 퀘스트 버튼에 쓸 텍스트 (예: "버섯 채집 퀘스트").</summary>
    public string QuestButtonText = "퀘스트";
}