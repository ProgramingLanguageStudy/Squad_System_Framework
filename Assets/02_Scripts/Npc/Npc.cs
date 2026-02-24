using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private string _npcId;

    public void Interact(IInteractReceiver receiver)
    {
        if (!string.IsNullOrEmpty(_npcId))
            PlaySceneServices.EventHub?.RaiseNpcInteracted(_npcId);
    }

    public string GetInteractText() => _npcId;
}
