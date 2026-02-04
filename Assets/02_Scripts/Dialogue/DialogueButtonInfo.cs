using UnityEngine;

public enum DialogueActionType
{
    End,            // end dialogue
    QuestAccept,    // accept quest
    QuestComplete,  // after submit
    GiveGift,       // later: gift
}

public struct DialogueButtonInfo
{
    public string Label;                  // display text
    public DialogueActionType ActionType; // action on click
    public Sprite Icon;                   // optional
}