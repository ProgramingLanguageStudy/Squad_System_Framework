using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }

    [SerializeField] private DialogueUI _ui;
    private FlagManager _flagManager;

    private void Awake() => Instance = this; // 단순화된 싱글톤

    public void Setup(FlagManager fm) => _flagManager = fm;

    public void StartDialogue(DialogueData data)
    {
        // 1. 현재 플래그 확인
        int currentStep = _flagManager.GetFlag(data.flagKey);

        // 2. 조건에 맞는 브랜치 찾기
        DialogueBranch activeBranch = data.branches.Find(b => b.requiredStep == currentStep);

        if (activeBranch.sentences != null)
        {
            // 3. UI에 전달 (대화 종료 후 플래그 업데이트 로직 포함 가능)
            _ui.Open(data.npcName, activeBranch.sentences);
            _flagManager.SetFlag(data.flagKey, activeBranch.nextStepValue);
        }
    }
}