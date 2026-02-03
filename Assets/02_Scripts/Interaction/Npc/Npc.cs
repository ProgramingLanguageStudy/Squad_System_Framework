using UnityEngine;

public class Npc : MonoBehaviour, IInteractable
{
    // 이제 에셋을 직접 할당할 필요 없이, 엑셀에 적은 NpcId만 적어주면 됩니다.
    [SerializeField] private string npcId;

    public string GetInteractText()
    {
        return $"{npcId}와 대화하기[E]";
    }

    public void Interact(Player player)
    {
        DialogueData data = DialogueManager.Instance.GetBestDialogue(npcId);

        if (data == null) return; // 데이터가 없을 경우를 대비한 안전장치

        // 문장들을 쪼개고, 각 문장 앞뒤에 붙었을지 모를 불필요한 공백을 제거합니다.
        string[] sentences = data.Sentence.Split('/');
        for (int i = 0; i < sentences.Length; i++)
        {
            sentences[i] = sentences[i].Trim();
        }

        // AfterActionEvent가 있다면 넘겨주는 로직도 나중에 여기에 추가될 수 있겠죠?
        DialogueSystem.Instance.StartDialogue(npcId, sentences);
        Debug.Log($"{npcId} {sentences.Length}");
    }
}