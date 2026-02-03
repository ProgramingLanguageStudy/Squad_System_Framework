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
                // ������ �ش� Ÿ���� ã��
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    // ���� ���ٸ� ���� ���� (���� ����)
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
            // ���� �ٲ� �����ϰ� �ʹٸ� �Ʒ� �ּ� ����
            // DontDestroyOnLoad(gameObject); 
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}