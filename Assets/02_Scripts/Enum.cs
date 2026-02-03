public enum QuestType { Gather, Kill, Visit } // 수집, 처치, 방문
public enum QuestState { CanStart, InProgress, CanComplete, Completed }

public enum DialogueType
{
    FirstMeet,  // 첫 대화용. first_talk_플래그가 0일 때만 선택
    Quest,
    Common,
    Affection,
}
