using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct DialogueBranch
{
    public int requiredStep;    // 이 대사가 나오기 위한 플래그 단계
    public string[] sentences;  // 실제 대사 내용
    public int nextStepValue;   // 대화 종료 후 플래그를 몇 단계로 올릴 것인가?
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Data")]
public class DialogueData : ScriptableObject
{
    public string npcName;      // NPC 이름
    public string flagKey;      // 확인할 플래그 이름 (예: "Quest_Chief")
    public List<DialogueBranch> branches; // 단계별 대사 묶음
}