using UnityEngine;

/// <summary>
/// Resources.Load 기반 로더. 추후 AddressablesResourceLoader로 교체 가능.
/// pathOrAddress = Resources 폴더 하위 경로 (예: "Prefabs/ItemObject")
/// </summary>
public class ResourcesResourceLoader : IResourceLoader
{
    public GameObject LoadPrefab(string pathOrAddress)
    {
        return Load<GameObject>(pathOrAddress);
    }

    public T Load<T>(string pathOrAddress) where T : Object
    {
        if (string.IsNullOrEmpty(pathOrAddress)) return null;
        return Resources.Load<T>(pathOrAddress);
    }
}
