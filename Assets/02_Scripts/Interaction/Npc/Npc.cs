using UnityEngine;
using System.Linq;

public class Npc : MonoBehaviour, IInteractable
{
    [SerializeField] private string _npcId;

    // �׽�Ʈ�� ���� �ν����Ϳ��� ���� ������ Ȯ���ϰ� �ʹٸ� �Ʒ�ó�� �ۼ� �����մϴ�.
    [Header("Debug Info")]
    [SerializeField] private int _currentAffectionDebug;

    private void Update()
    {
        // ���� 1��: ȣ���� ��� / 2��: ȣ���� �϶�
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
        // �Ŵ����� ��ųʸ��� ������ ���� ������ ��縦 ã���ݴϴ�.
        DialogueData data = DialogueManager.Instance.GetBestDialogue(_npcId);

        if (data == null) return;

        // ��� ����
        string[] sentences = data.Sentence.Split('/')
                                .Select(s => s.Trim())
                                .ToArray();

        // ��ȭ �ý��� ����
        DialogueSystem.Instance.StartDialogue(_npcId, sentences);
    }

    public string GetInteractText() => _npcId;
}