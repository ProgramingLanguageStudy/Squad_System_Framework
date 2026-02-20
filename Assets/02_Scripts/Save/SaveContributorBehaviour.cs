using UnityEngine;

/// <summary>
/// ISaveContributor를 구현하는 MonoBehaviour 베이스. FindObjectsByType으로 수집 가능.
/// </summary>
public abstract class SaveContributorBehaviour : MonoBehaviour, ISaveContributor
{
    public abstract int SaveOrder { get; }
    public abstract void Gather(SaveData data);
    public abstract void Apply(SaveData data);
}
