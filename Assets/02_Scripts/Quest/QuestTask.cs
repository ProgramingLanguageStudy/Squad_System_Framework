[System.Serializable]
public abstract class QuestTask
{
    public string Description; // UI에 표시될 목표 문구 (예: 버섯 3개 수집)
    public int TargetAmount;   // 목표 수치
    public int CurrentAmount;  // 현재 진행 수치

    // 목표를 달성했는지 체크하는 공통 로직
    public bool IsCompleted => CurrentAmount >= TargetAmount;

    // "나 버섯 1개 먹었어" 같은 신호를 받았을 때 수치를 올리는 함수
    public abstract void UpdateProgress(string id, int amount);
}