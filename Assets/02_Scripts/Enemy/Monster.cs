using UnityEngine;

/// <summary>
/// 몬스터 = Model 보유 컨테이너. 전투/체력바에서 Monster.Model로 참조.
/// </summary>
[RequireComponent(typeof(MonsterModel))]
public class Monster : MonoBehaviour
{
    [SerializeField] private MonsterModel _model;

    public MonsterModel Model => _model;

    private void Awake()
    {
        if (_model == null) _model = GetComponent<MonsterModel>();
    }

    private void Start()
    {
        _model?.Initialize();
    }
}
