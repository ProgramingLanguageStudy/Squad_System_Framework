/// <summary>
/// 플래그 키 이름 규칙. 오타 방지·이름 변경 시 한 곳만 수정하려고 사용.
/// FlagManager.SetFlag(GameStateKeys.FirstTalkNpc("촌장"), 1) 처럼 쓰면 됨.
/// </summary>
public static class GameStateKeys
{
    private const string FirstTalkPrefix = "first_talk_";
    private const string AffectionPrefix = "affection_";
    private const string QuestPrefix = "quest_";

    /// <summary>해당 NPC와 첫 대화를 했는지. 0=아직, 1=했음.</summary>
    public static string FirstTalkNpc(string npcId) => FirstTalkPrefix + npcId;

    /// <summary>호감도. 값이 숫자로 저장됨.</summary>
    public static string Affection(string npcId) => AffectionPrefix + npcId;

    /// <summary>퀘스트 수락 여부. 예: quest_버섯5개_수락</summary>
    public static string QuestAccepted(string questId) => QuestPrefix + questId + "_수락";

    /// <summary>퀘스트 완료 여부. 예: quest_버섯5개_완료</summary>
    public static string QuestCompleted(string questId) => QuestPrefix + questId + "_완료";
}
