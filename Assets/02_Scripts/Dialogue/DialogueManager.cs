using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueManager : Singleton<DialogueManager>
{
    // CSV���� ��ȯ�� ��� DialogueData ���µ��� ��Ƶδ� ����Ʈ
    private List<DialogueData> _dialogueDatabase = new List<DialogueData>();

    // NPC ID�� ȣ���� ������ �����ϴ� ��ųʸ� (�߿�!)
    private Dictionary<string, int> _affectionTable = new Dictionary<string, int>();

    // �ε� �Ϸ� ���θ� Ȯ���ϱ� ���� ������Ƽ
    public bool IsLoaded { get; private set; } = false;

    protected override void Awake()
    {
        base.Awake(); // �̱��� �ʱ�ȭ ����
        LoadAllDialogueAssets();
    }

    /// <summary>
    /// Resources/Dialogues ���� ���� ��� DialogueData ������ �޸𸮷� �ε��մϴ�.
    /// </summary>
    private void LoadAllDialogueAssets()
    {
        DialogueData[] assets = Resources.LoadAll<DialogueData>("Dialogues");

        if (assets == null || assets.Length == 0)
        {
            Debug.LogWarning("<color=yellow>[DialogueManager]</color> �ε��� ��� ������ �����ϴ�.");
            return;
        }

        _dialogueDatabase = assets.ToList();
        IsLoaded = true;

        Debug.Log($"<color=cyan>[DialogueManager]</color> �� {_dialogueDatabase.Count}���� ��� �����͸� �ε��߽��ϴ�.");
    }

    /// <summary>
    /// NPC ID�� ������� ���� ��Ȳ�� ���� ������ ��� �����͸� ��ȯ�մϴ�.
    /// </summary>
    public DialogueData GetBestDialogue(string npcId)
    {
        // 1. �ش� NPC�� ��� ��� ���͸�
        var npcTalks = _dialogueDatabase.Where(d => d.NpcId == npcId).ToList();

        if (npcTalks.Count == 0)
        {
            Debug.LogError($"[DialogueManager] {npcId}�� �ش��ϴ� ��� �����Ͱ� �����ϴ�.");
            return null;
        }

        // 2. ���� NPC�� ȣ���� ���� ��������
        int currentAffection = GetAffection(npcId);

        // 3. �켱���� �Ǻ�

        // 3-1. ����Ʈ ��� (���߿� QuestManager ���� �� Ȱ��ȭ)
        // DialogueData questTalk = npcTalks.FirstOrDefault(t => t.DialogueType == DialogueType.Quest && CheckQuest(t));
        // if (questTalk != null) return questTalk;

        // 3-2. ȣ���� ��� (���� ���� ������ �䱸ġ �� ���� ���� ���� ����)
        var affectionTalk = npcTalks
            .Where(t => t.DialogueType == DialogueType.Affection)
            .Where(t => t.ConditionValue <= currentAffection)
            .OrderByDescending(t => t.ConditionValue)
            .FirstOrDefault();

        if (affectionTalk != null) return affectionTalk;

        // 3-3. �⺻ ��� (Common) �� ���� ��ȯ
        var commonTalks = npcTalks.Where(t => t.DialogueType == DialogueType.Common).ToList();
        if (commonTalks.Count > 0)
        {
            return commonTalks[Random.Range(0, commonTalks.Count)];
        }

        return npcTalks[0];
    }

    // --- ȣ���� ���� API ---

    /// <summary>
    /// NPC�� ���� ȣ���� ������ ��ȯ�մϴ�.
    /// </summary>
    public int GetAffection(string npcId)
    {
        if (_affectionTable.ContainsKey(npcId))
            return _affectionTable[npcId];
        return 0;
    }

    /// <summary>
    /// NPC�� ȣ������ ������ŵ�ϴ�.
    /// </summary>
    public void AddAffection(string npcId, int amount)
    {
        if (!_affectionTable.ContainsKey(npcId))
            _affectionTable[npcId] = 0;

        _affectionTable[npcId] += amount;
        Debug.Log($"<color=magenta>[Affection]</color> {npcId}�� ���� ȣ����: {_affectionTable[npcId]}");
    }
}