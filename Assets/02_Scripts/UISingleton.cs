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
                // FindFirstObjectByType�� �����ִ� ������Ʈ�� �� ã�� ���� �����ϴ�.
                // �Ʒ� �ɼ��� ������ �����ִ�(Inactive) ������Ʈ���� �� �� ������ ã�Ƴ��ϴ�.
                _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);

                if (_instance == null)
                {
                    Debug.LogError($"[UISingleton] {typeof(T).Name}�� ã�� �� �����ϴ�!");
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
            // UI�� ���� Ư�� ĵ���� �ͼ��̶� DontDestroyOnLoad�� �����Դϴ�.
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}