using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public static DialogueSystem Instance { get; private set; }
    public bool IsTalking { get; set; }
    [SerializeField] private DialogueUI _ui;

    private DialogueData _currentData;
    private QuestDialogueBranch _currentBranch;
    private bool _isQuestDialogue;

    private void Awake() => Instance = this;

    public void StartDialogue(Npc npc)
    {
        IsTalking = true;

        Debug.Log($"{npc.NpcName}와 대화 시작 시도");
        _currentData = npc.GetCurrentDialogue();

        if (_currentData == null)
        {
            Debug.LogError("대본 데이터가 없습니다!");
            return;
        }

        // 이름 결정 (대본 이름 우선, 없으면 NPC 이름)
        string displayName = string.IsNullOrEmpty(_currentData.NpcName) ? npc.NpcName : _currentData.NpcName;

        QuestState state = QuestManager.Instance.GetQuestState(_currentData.QuestKey);
        Debug.Log($"현재 퀘스트 상태: {state}");

        _currentBranch = _currentData.QuestBranches.Find(b => b.RequiredState == state);

        // 1. 퀘스트 대사 출력
        if (_currentBranch.Sentences != null && _currentBranch.Sentences.Length > 0)
        {
            Debug.Log("퀘스트 대사 출력!");
            _isQuestDialogue = true;
            _ui.Open(displayName, _currentBranch.Sentences);
        }
        // 2. 일상 대사 출력
        else if (_currentData.NormalGroups.Count > 0)
        {
            Debug.Log("일상 대사 출력 시도!");
            _isQuestDialogue = false;
            int rand = Random.Range(0, _currentData.NormalGroups.Count);
            _ui.Open(displayName, _currentData.NormalGroups[rand].Sentences);
        }
    }

    // 대화가 끝났을 때 UI에서 호출됨
    public void OnDialogueComplete()
    {
        IsTalking = false;
        
        if (_isQuestDialogue && _currentBranch.NextStepValue != -1)
        {
            QuestManager.Instance.SetQuestStep(_currentData.QuestKey, _currentBranch.NextStepValue);
        }
    }
}