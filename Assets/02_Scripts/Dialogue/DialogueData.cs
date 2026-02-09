using UnityEngine;

/// <summary>
/// 대화 한 편: NPC·타입(FirstTalk/Quest/Common 등)·선택 필드(questId, conditionValue)로 한 SO에 정의.
/// 상속 없이 dialogueType + optional 필드로 구분.
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Data")]
public class DialogueData : ScriptableObject
{
    [Tooltip("대화 블록 구분용 ID. GetById 등에서 사용")]
    public string id;

    [Header("선택 조건")]
    [Tooltip("이 대화를 쓰는 NPC ID")]
    public string npcId;
    [Tooltip("대화 종류: FirstTalk(첫 대화), Quest(수락), QuestComplete(완료), Common(일상), Affection(호감도)")]
    public DialogueType dialogueType;
    [Tooltip("Quest/QuestComplete일 때 퀘스트 ID. 그 외는 비워둠")]
    public string questId;
    [Tooltip("Affection일 때 최소 호감도. 그 외는 0")]
    public int conditionValue;

    [Header("내용")]
    [Tooltip("한 문장씩 순서대로 재생 (독백)")]
    [TextArea(1, 3)]
    public string[] lines;

    /// <summary>빈 문장 제거·null 방지한 복사. 없으면 길이 0 배열.</summary>
    public string[] Lines
    {
        get
        {
            if (lines == null || lines.Length == 0) return System.Array.Empty<string>();
            var list = new System.Collections.Generic.List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                var s = lines[i] != null ? lines[i].Trim() : "";
                if (s.Length > 0) list.Add(s);
            }
            return list.Count > 0 ? list.ToArray() : System.Array.Empty<string>();
        }
    }
}
