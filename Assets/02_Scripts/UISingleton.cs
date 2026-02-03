using UnityEngine;

public class UISingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // FindFirstObjectByType은 비활성화된 오브젝트는 못 찾을 수 있습니다.
                // 아래 옵션을 넣으면 비활성화된(Inactive) 오브젝트까지 찾을 수 있습니다.
                _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);

                if (_instance == null)
                {
                    Debug.LogError($"[UISingleton] {typeof(T).Name}을 찾을 수 없습니다!");
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            // UI의 경우 특정 씬에만 있다고 가정해서 DontDestroyOnLoad는 사용하지 않습니다.
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
