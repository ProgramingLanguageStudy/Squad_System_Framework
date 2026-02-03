using UnityEngine;
using System.Linq;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private string _npcId;

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
        DialogueData data = DialogueManager.Instance.GetBestDialogue(_npcId);
        if (data == null) return;

        string[] sentences = data.Sentence.Split('/').Select(s => s.Trim()).ToArray();

        string questButtonText = "퀘스트";
        bool showQuestButton = data.DialogueType == DialogueType.Common
            && DialogueManager.Instance.GetAvailableQuestForNpc(_npcId, out questButtonText);

        System.Action onDialogueEnd = () =>
        {
            var flagManager = FindFirstObjectByType<FlagManager>();
            if (flagManager != null)
                flagManager.SetFlag(GameStateKeys.FirstTalkNpc(_npcId), 1);
        };

        DialogueSystem.Instance.StartDialogue(_npcId, sentences, _npcId, data.DialogueType, showQuestButton, questButtonText ?? "퀘스트", onDialogueEnd);
    }

    public string GetInteractText() => _npcId;
}
