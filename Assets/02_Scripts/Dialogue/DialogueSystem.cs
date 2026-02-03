using System;
using UnityEngine;

public class DialogueSystem : Singleton<DialogueSystem>
{
    [SerializeField] private DialogueUI _ui;
    public bool IsTalking { get; private set; }
    private Action _onDialogueComplete;

    protected override void Awake()
    {
        base.Awake();
    }

    public void StartDialogue(string speakerName, string[] sentences, Action onComplete = null)
    {
        if (IsTalking) return;
        IsTalking = true;
        _onDialogueComplete = onComplete;
        _ui.Open(speakerName, sentences);
        Debug.Log($"{speakerName} {sentences}");
    }

    public void DisplayNextSentence()
    {
        if (!IsTalking) return;
        if (_ui.ShowNext()) EndDialogue();
    }

    public void EndDialogue()
    {
        if (!IsTalking) return;
        IsTalking = false;
        _ui.Close();
        _onDialogueComplete?.Invoke();
        _onDialogueComplete = null;
    }
}