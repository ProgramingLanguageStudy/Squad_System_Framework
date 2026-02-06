public enum QuestType { Gather, Kill, Visit } // 수집, 처치, 방문

public enum DialogueType
{
    FirstMeet,  // 첫 대화용. first_talk_플래그가 0일 때만 선택
    Quest,
    QuestComplete, // 퀘스트 제출 후 완료 대사
    Common,
    Affection,
}
