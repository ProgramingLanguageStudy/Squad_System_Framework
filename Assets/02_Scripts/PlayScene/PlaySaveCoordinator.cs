using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Play 씬 세이브/로드 조율. ISaveHandler 구현, DataManager에 등록.
/// PlayScene.Initialize에서 FindObjectsOfType으로 수집한 Contributor들을 주입받아 Gather/Apply 시 분배.
/// PlayScene에 있을 필요 없음. 씬 어디든 둬도 됨.
/// </summary>
public class PlaySaveCoordinator : MonoBehaviour, ISaveHandler
{
    private List<ISaveContributor> _contributors = new List<ISaveContributor>();

    public void Initialize(IEnumerable<ISaveContributor> contributors)
    {
        _contributors = contributors != null ? contributors.ToList() : new List<ISaveContributor>();
    }

    private void OnEnable()
    {
        if (GameManager.Instance?.DataManager != null)
            GameManager.Instance.DataManager.Register(this);
    }

    private void OnDisable()
    {
        if (GameManager.Instance?.DataManager != null)
            GameManager.Instance.DataManager.Unregister(this);
    }

    public void Gather(SaveData data)
    {
        if (data == null) return;
        foreach (var c in _contributors.OrderBy(x => x.SaveOrder))
            c.Gather(data);
    }

    public void Apply(SaveData data)
    {
        if (data == null) return;
        foreach (var c in _contributors.OrderBy(x => x.SaveOrder))
            c.Apply(data);
    }
}
