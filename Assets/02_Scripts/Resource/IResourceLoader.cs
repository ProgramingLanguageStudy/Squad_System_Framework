using UnityEngine;

/// <summary>
/// 에셋 로드 인터페이스. Resources.Load 또는 Addressables 구현 교체용.
/// </summary>
public interface IResourceLoader
{
    /// <summary>경로/주소로 GameObject 프리팹 로드.</summary>
    GameObject LoadPrefab(string pathOrAddress);

    /// <summary>제네릭 로드. T = GameObject, Sprite, AudioClip 등.</summary>
    T Load<T>(string pathOrAddress) where T : Object;
}
