using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 세이브 데이터 포함 추후 데이터 관리. 각 시스템이 OnEnable에서 Set(등록), OnDisable에서 Unset(해제).
/// SaveManager가 저장/로드 시점에 DataManager.GatherSaveData / ApplySaveData 사용.
/// </summary>
public class DataManager : MonoBehaviour
{
    private readonly List<ISaveHandler> _handlers = new List<ISaveHandler>();

    /// <summary>세이브/로드에 참여할 핸들러 등록. 각 시스템은 OnEnable에서 호출.</summary>
    public void Register(ISaveHandler handler)
    {
        if (handler == null || _handlers.Contains(handler)) return;
        _handlers.Add(handler);
    }

    /// <summary>등록 해제. 각 시스템은 OnDisable에서 호출.</summary>
    public void Unregister(ISaveHandler handler)
    {
        if (handler == null) return;
        _handlers.Remove(handler);
    }

    /// <summary>등록된 핸들러들로부터 SaveData 수집. SaveManager가 저장 직전에 호출.</summary>
    public SaveData GatherSaveData()
    {
        var data = new SaveData();
        for (int i = 0; i < _handlers.Count; i++)
            _handlers[i].Gather(data);
        return data;
    }

    /// <summary>로드한 SaveData를 등록된 핸들러들에게 적용. SaveManager가 로드 직후에 호출.</summary>
    public void ApplySaveData(SaveData data)
    {
        if (data == null) return;
        for (int i = 0; i < _handlers.Count; i++)
            _handlers[i].Apply(data);
    }
}
