using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private string _npcId;

    public void Interact(Player player)
    {
        // 대화 시작은 나중에 DialogueInteractor 등에서 GameEvents.OnPlayDialogueRequested(data) 로 연결
    }

    public string GetInteractText() => _npcId;
}
