using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private string _npcId;
    [SerializeField] private DialogueCoordinator _dialogueCoordinator;

    [Header("Debug Info")]
    [SerializeField] private int _currentAffectionDebug;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DialogueManager.Instance.AddAffection(_npcId, 10);
            _currentAffectionDebug = DialogueManager.Instance.GetAffection(_npcId);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DialogueManager.Instance.AddAffection(_npcId, -10);
            _currentAffectionDebug = DialogueManager.Instance.GetAffection(_npcId);
        }
    }

    public void Interact(Player player)
    {
        var coordinator = _dialogueCoordinator != null ? _dialogueCoordinator : FindFirstObjectByType<DialogueCoordinator>();
        if (coordinator != null)
            coordinator.StartDialogue(_npcId);
    }

    public string GetInteractText() => _npcId;
}
