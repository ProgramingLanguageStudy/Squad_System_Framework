using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private string _npcId;

    public void Interact(Player player)
    {
        if (!string.IsNullOrEmpty(_npcId))
            GameEvents.OnNpcInteracted?.Invoke(_npcId);
    }

    public string GetInteractText() => _npcId;
}
