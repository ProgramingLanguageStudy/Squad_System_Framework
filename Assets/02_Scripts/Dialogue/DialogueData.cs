using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Data")]
public class DialogueData : ScriptableObject
{
    public string NpcId;
    public DialogueType DialogueType;
    public string ConditionKey = string.Empty;
    public int ConditionValue;
    [TextArea(3, 5)] public string Sentence;

    public string AfterActionEvent = string.Empty;
}