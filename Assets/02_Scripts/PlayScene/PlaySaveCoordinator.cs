using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Play 씬 세이브/로드 조율. ISaveHandler 구현, DataManager에 등록.
/// 인스펙터에서 Contributor 할당. Gather/Apply 시 분배.
/// </summary>
public class PlaySaveCoordinator : MonoBehaviour, ISaveHandler
{
    [SerializeField] [Tooltip("세이브/로드에 참여할 Contributor. SaveOrder 순으로 처리")]
    private List<SaveContributorBehaviour> _contributors = new List<SaveContributorBehaviour>();

    public void Initialize() { }

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
        if (data == null || _contributors == null) return;
        foreach (var c in _contributors.Where(x => x != null).OrderBy(x => x.SaveOrder))
            c.Gather(data);
    }

    public void Apply(SaveData data)
    {
        if (data == null || _contributors == null) return;
        foreach (var c in _contributors.Where(x => x != null).OrderBy(x => x.SaveOrder))
            c.Apply(data);
    }
}
