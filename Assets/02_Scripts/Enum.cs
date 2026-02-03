public enum QuestState
{
    NotStarted, // 0 (기본값)
    InProgress, // 1
    GoalMet,    // 2
    Completed,  // 3
    None        // 4 (예외 처리용)
}

public enum DialogueType
{
    Quest,
    Common,
    Affection,
}