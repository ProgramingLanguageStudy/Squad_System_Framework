using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대화 데이터 로드·조회만. 대화 시스템과 분리. NPC/조율층에서 "어떤 대화 쓸지" 정할 때 사용.
/// </summary>
public class DialogueDataLoader : MonoBehaviour
{
    private Dictionary<string, DialogueData> _byId = new Dictionary<string, DialogueData>();
    private Dictionary<string, List<DialogueData>> _byNpcId = new Dictionary<string, List<DialogueData>>();
    public bool IsLoaded { get; private set; }

    private void Awake()
    {
        StartCoroutine(LoadNextFrame());
    }

    private IEnumerator LoadNextFrame()
    {
        yield return null;
        LoadAll();
    }

    private void LoadAll()
    {
        var assets = Resources.LoadAll<DialogueData>("Dialogues");
        _byId.Clear();
        _byNpcId.Clear();
        if (assets != null)
        {
            for (int i = 0; i < assets.Length; i++)
            {
                var d = assets[i];
                if (d == null) continue;
                if (!string.IsNullOrEmpty(d.id)) _byId[d.id] = d;
                if (!string.IsNullOrEmpty(d.npcId))
                {
                    if (!_byNpcId.TryGetValue(d.npcId, out var list))
                    {
                        list = new List<DialogueData>();
                        _byNpcId[d.npcId] = list;
                    }
                    list.Add(d);
                }
            }
        }
        IsLoaded = true;
    }

    public DialogueData GetById(string id)
    {
        return IsLoaded && _byId.TryGetValue(id, out var data) ? data : null;
    }

    /// <summary>FirstTalk 우선, 없으면 Common 중 랜덤, 없으면 첫 번째.</summary>
    public DialogueData GetBestForNpc(string npcId)
    {
        if (!IsLoaded || !_byNpcId.TryGetValue(npcId, out var list) || list.Count == 0)
            return null;
        for (int i = 0; i < list.Count; i++)
            if (list[i].dialogueType == DialogueType.FirstTalk) return list[i];
        int commonCount = 0;
        for (int i = 0; i < list.Count; i++)
            if (list[i].dialogueType == DialogueType.Common) commonCount++;
        if (commonCount > 0)
        {
            int pick = Random.Range(0, commonCount);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].dialogueType != DialogueType.Common) continue;
                if (pick-- == 0) return list[i];
            }
        }
        return list[0];
    }
}
