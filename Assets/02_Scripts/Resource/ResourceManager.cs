using UnityEngine;

/// <summary>
/// 에셋 로드 담당. IResourceLoader 주입. 기본은 ResourcesResourceLoader.
/// Addressables 도입 시 AddressablesResourceLoader 구현 후 교체.
/// GameManager 하위 또는 독립 배치.
/// </summary>
public class ResourceManager : MonoBehaviour
{
    private IResourceLoader _loader;

    private IResourceLoader Loader => _loader ??= new ResourcesResourceLoader();

    /// <summary>로더 교체. Addressables 전환 시 호출.</summary>
    public void SetLoader(IResourceLoader loader) => _loader = loader;

    public GameObject LoadPrefab(string pathOrAddress) => Loader.LoadPrefab(pathOrAddress);

    public T Load<T>(string pathOrAddress) where T : Object => Loader.Load<T>(pathOrAddress);
}
