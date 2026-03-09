using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임오브젝트 풀. 미리 생성해 두었다가 Pop/Push로 재사용.
/// </summary>
public class Pool
{
    private readonly Stack<GameObject> _pool;
    private readonly GameObject _prefab;
    private readonly Transform _parent;

    public Pool(GameObject prefab, Transform parent, int initialSize)
    {
        _prefab = prefab;
        _parent = parent;
        _pool = new Stack<GameObject>(initialSize);

        for (int i = 0; i < initialSize; i++)
            CreateAndPush();
    }

    private void CreateAndPush()
    {
        var go = Object.Instantiate(_prefab, _parent);
        go.SetActive(false);

        var poolable = go.GetComponent<Poolable>();
        if (poolable == null)
            poolable = go.AddComponent<Poolable>();
        poolable.SetPool(this);

        _pool.Push(go);
    }

    public GameObject Pop()
    {
        if (_pool.Count > 0)
        {
            var go = _pool.Pop();
            go.SetActive(true);
            return go;
        }

        var newGo = Object.Instantiate(_prefab, _parent);
        var p = newGo.GetComponent<Poolable>();
        if (p == null) p = newGo.AddComponent<Poolable>();
        p.SetPool(this);
        return newGo;
    }

    public void Push(GameObject go)
    {
        if (go == null) return;
        go.transform.SetParent(_parent);
        go.SetActive(false);
        _pool.Push(go);
    }
}
