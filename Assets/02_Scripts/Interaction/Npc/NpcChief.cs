using UnityEngine;

public class NpcChief : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueData _dialogueData;

    public string GetInteractText() => $"{_dialogueData.npcName}과 대화하기 (E)";

    public void Interact(Player player)
    {
        // "내 데이터 여기 있으니 시스템이 알아서 해줘!"
        DialogueSystem.Instance.StartDialogue(_dialogueData);
    }
}