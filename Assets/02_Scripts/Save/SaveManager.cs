using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 세이브/로드 시점 제어 및 API 제공. DataManager에서 데이터 수집·적용, SaveLoadSystem에서 I/O.
/// - Play 진입 시: 로드 후 DataManager.ApplySaveData.
/// - Play 종료 시(에디터): 저장.
/// - 앱/에디터 종료 시: 저장.
/// GameManager와 연결되어 있으며, GameManager가 보유.
/// </summary>
public class SaveManager : MonoBehaviour
{
    [SerializeField] private int _defaultSlot = 0;

    private void Start()
    {
        // OnEnable에서 핸들러 등록이 끝난 뒤 로드하도록 한 프레임 지연
        StartCoroutine(LoadAndApplyNextFrame());
    }

    private System.Collections.IEnumerator LoadAndApplyNextFrame()
    {
        yield return null;
        LoadAndApply();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
            Save();
    }
#endif

    /// <summary>현재 게임 상태를 DataManager로 수집 후 저장. 외부 API.</summary>
    public void Save()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        var dataManager = gm.DataManager;
        var saveLoadSystem = gm.SaveLoadSystem;
        if (dataManager == null || saveLoadSystem == null) return;

        var data = dataManager.GatherSaveData();
        if (data == null) return;

        if (saveLoadSystem.Save(_defaultSlot, data))
            Debug.Log("[SaveManager] Saved to slot " + _defaultSlot);
        else
            Debug.LogWarning("[SaveManager] Save failed.");
    }

    /// <summary>슬롯에서 로드 후 DataManager로 적용. 외부 API.</summary>
    public void LoadAndApply()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        var dataManager = gm.DataManager;
        var saveLoadSystem = gm.SaveLoadSystem;
        if (dataManager == null || saveLoadSystem == null) return;

        var data = saveLoadSystem.Load(_defaultSlot);
        if (data == null)
        {
            Debug.Log("[SaveManager] No save in slot " + _defaultSlot + ", skip load.");
            return;
        }

        dataManager.ApplySaveData(data);
        Debug.Log("[SaveManager] Loaded from slot " + _defaultSlot);
    }
}
