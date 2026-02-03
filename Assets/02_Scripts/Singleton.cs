using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에서 해당 타입을 찾음
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    // 씬에 없다면 새로 생성 (선택 사항)
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
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
            // 씬이 바뀌어도 유지하고 싶다면 아래 주석 해제
            // DontDestroyOnLoad(gameObject); 
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}