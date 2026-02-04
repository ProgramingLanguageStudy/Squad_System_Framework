using UnityEngine;
using System.Linq;
using System;

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
        bool showQuestButton = false;
        bool showSubmitButton = false;
        string submitButtonText = "제출하기";

        // 일상·호감도 대사일 때만 퀘스트/제출 버튼 표시 (Common 또는 Affection)
        if (data.DialogueType == DialogueType.Common || data.DialogueType == DialogueType.Affection)
        {
            if (DialogueManager.Instance.GetCompletableQuestForNpc(_npcId, out _, out _, out submitButtonText))
                showSubmitButton = true;
            else if (DialogueManager.Instance.GetAvailableQuestForNpc(_npcId, out questButtonText))
                showQuestButton = true;
        }

        Action onDialogueEnd = () =>
        {
            var flagManager = FindFirstObjectByType<FlagManager>();
            if (flagManager != null)
                flagManager.SetFlag(GameStateKeys.FirstTalkNpc(_npcId), 1);
        };

        DialogueSystem.Instance.StartDialogue(_npcId, sentences, _npcId, data.DialogueType,
            showQuestButton, questButtonText ?? "퀘스트", onDialogueEnd, showSubmitButton, submitButtonText);
    }

    public string GetInteractText() => _npcId;
}
