using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 데이터 preload·관리. DialogueData 등 Resources 로드 후 npcId 기준 매핑.
/// GameManager가 보유.
/// </summary>
public class DataManager : MonoBehaviour
{
    private Dictionary<string, List<DialogueData>> _dialoguesByNpcId = new Dictionary<string, List<DialogueData>>();

    public bool IsLoaded { get; private set; }

    /// <summary>GameManager에서 호출. 중복 호출 시 스킵.</summary>
    public void Initialize()
    {
        if (IsLoaded) return;
        LoadDialogues();
    }

    private void LoadDialogues()
    {
        _dialoguesByNpcId.Clear();
        var assets = Resources.LoadAll<DialogueData>("Dialogues");

        if (assets != null)
        {
            for (int i = 0; i < assets.Length; i++)
            {
                var d = assets[i];
                if (d == null || string.IsNullOrEmpty(d.npcId)) continue;

                if (!_dialoguesByNpcId.TryGetValue(d.npcId, out var list))
                {
                    list = new List<DialogueData>();
                    _dialoguesByNpcId[d.npcId] = list;
                }
                list.Add(d);
            }
        }

        IsLoaded = true;
    }

    /// <summary>해당 npcId의 대화 목록. 없으면 빈 리스트.</summary>
    public IReadOnlyList<DialogueData> GetDialoguesForNpc(string npcId)
    {
        if (string.IsNullOrEmpty(npcId) || !_dialoguesByNpcId.TryGetValue(npcId, out var list))
            return Array.Empty<DialogueData>();
        return list;
    }
}
