using UnityEngine;

/// <summary>
/// 게임 전역에서 유일한 싱글톤. 씬 전환 후에도 유지되며, 다른 매니저/시스템을 보유합니다.
/// 접근: GameManager.Instance.SaveManager, GameManager.Instance.DataManager, GameManager.Instance.SaveLoadSystem
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [Header("Managers & Systems (선택: 인스펙터 할당. 없으면 런타임 생성)")]
    [SerializeField] private SaveManager _saveManager;
    [SerializeField] private DataManager _dataManager;

    /// <summary>세이브/로드 시점·API. GameManager와 연결된 진입점.</summary>
    public SaveManager SaveManager => GetOrCreate(ref _saveManager, "SaveManager");
    /// <summary>세이브 데이터 등 추후 데이터 관리. 각 시스템이 Set으로 등록.</summary>
    public DataManager DataManager => GetOrCreate(ref _dataManager, "DataManager");
    /// <summary>세이브/로드 I/O 시스템.</summary>
    public SaveLoadSystem SaveLoadSystem => GetOrCreate(ref _saveLoadSystem, "SaveLoadSystem");

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>매니저가 없으면 자식 오브젝트에서 찾거나, 없으면 생성해서 붙임.</summary>
    private T GetOrCreate<T>(ref T field, string childName) where T : MonoBehaviour
    {
        if (field != null) return field;

        var existing = GetComponentInChildren<T>(true);
        if (existing != null)
        {
            field = existing;
            return field;
        }

        var go = new GameObject(childName);
        go.transform.SetParent(transform);
        field = go.AddComponent<T>();
        return field;
    }
}
