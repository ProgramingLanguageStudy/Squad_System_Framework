using System.Linq;
using UnityEngine;

/// <summary>
/// 대화 시작만 조율. DialogueManager·DialogueSystem만 사용. (플래그·퀘스트 연동은 나중에 조율층에서.)
/// </summary>
public class DialogueCoordinator : MonoBehaviour
{
    [SerializeField] private DialogueManager _dialogueManager;
    [SerializeField] private DialogueSystem _dialogueSystem;

    /// <summary>PlayScene 등에서 한 번 호출.</summary>
    public void Initialize(DialogueManager dm, DialogueSystem ds)
    {
        _dialogueManager = dm;
        _dialogueSystem = ds;
    }

    /// <summary>NPC와 대화 시작.</summary>
    public void StartDialogue(string npcId)
    {
        var data = _dialogueManager != null ? _dialogueManager.GetBestDialogue(npcId) : null;
        if (data == null) return;

        string[] sentences = data.Sentence.Split('/').Select(s => s.Trim()).ToArray();
        if (_dialogueSystem != null)
            _dialogueSystem.StartDialogue(npcId, sentences, npcId, data.DialogueType, null);
    }
}
