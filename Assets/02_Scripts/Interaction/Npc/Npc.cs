using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    public string NpcName;
    public DialogueData QuestDialogue;  // 퀘스트 대본
    public DialogueData CommonDialogue; // 일상 대본

    public string GetInteractText() => $"{NpcName}와 대화하기";

    public void Interact(Player player)
    {
        // 대화 시작
        Debug.Log("NPC와 대화 시도!");
        DialogueSystem.Instance.StartDialogue(this);
    }

    public DialogueData GetCurrentDialogue()
    {
        // 퀘스트가 있고, 완료되지 않았다면 퀘스트 대본 우선
        if (QuestDialogue != null && !string.IsNullOrEmpty(QuestDialogue.QuestKey))
        {
            QuestState state = QuestManager.Instance.GetQuestState(QuestDialogue.QuestKey);
            if (state != QuestState.Completed) return QuestDialogue;
        }
        return CommonDialogue;
    }
}