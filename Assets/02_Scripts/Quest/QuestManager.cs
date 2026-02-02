using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    private FlagManager _flagManager;

    public void Setup(FlagManager flagManager) => _flagManager = flagManager;

    private void Awake() => Instance = this;

    public QuestState GetQuestState(string key)
    {
        if (string.IsNullOrEmpty(key) || _flagManager == null) return QuestState.None;

        int step = _flagManager.GetFlag(key);
        return (QuestState)step; // enum과 int가 매칭되도록 설계함
    }

    public void SetQuestStep(string key, int value)
    {
        _flagManager.SetFlag(key, value);
    }

    // QuestManager.cs
    public void CheckProgress(string key, int currentCount)
    {
        // 5개라는 목표 수치는 나중에 데이터(SO)로 빼면 좋지만, 일단은 코드로!
        if (key == "Mushroom_001" && currentCount >= 5)
        {
            // 조건을 만족하면 상태를 'GoalMet'으로 변경!
            SetQuestStep(key, (int)QuestState.GoalMet);
            Debug.Log("<color=yellow>퀘스트 알림: 버섯을 모두 모았습니다!</color>");
        }
    }
}