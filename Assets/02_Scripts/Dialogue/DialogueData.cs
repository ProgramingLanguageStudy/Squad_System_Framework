using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Data")]
public class DialogueData : ScriptableObject
{
    public string NpcId;
    public DialogueType DialogueType;
    /// <summary>호감도 대사(Affection)일 때 필요한 최소 호감도.</summary>
    public int ConditionValue;
    [TextArea(3, 5)] public string Sentence;

    /// <summary>퀘스트 제시/완료 대사일 때 연결된 퀘스트 ID (Resources/Quests/{id}.asset).</summary>
    public string LinkedQuestId = string.Empty;
    /// <summary>대화창 퀘스트 버튼 문구 (예: "버섯 채집 퀘스트", "버섯 5개 제출하기").</summary>
    public string QuestButtonText = "퀘스트";
}