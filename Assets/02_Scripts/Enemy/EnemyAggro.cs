using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적(Enemy)의 어그로 관리. 100 기준, 거리별 누적, 10m 이탈 시 리셋.
/// </summary>
public class EnemyAggro : MonoBehaviour
{
    private const float AggroThreshold = 100f;
    private const float AggroLoseDistance = 10f;

    /// <summary>거리 1m→50, 3m→30 (가까울수록 높음). 그 사이 선형 보간.</summary>
    private static float AggroFromDistance(float distance)
    {
        if (distance <= 1f) return 50f;
        if (distance >= 3f) return 30f;
        return Mathf.Lerp(50f, 30f, (distance - 1f) / 2f);
    }

    private readonly Dictionary<Character, float> _aggroTable = new Dictionary<Character, float>();

    public float Threshold => AggroThreshold;
    public float LoseDistance => AggroLoseDistance;

    public void AddAggro(Character target, float amount)
    {
        if (target == null) return;
        if (!_aggroTable.ContainsKey(target)) _aggroTable[target] = 0f;
        _aggroTable[target] = Mathf.Max(0f, _aggroTable[target] + amount);
    }

    public void AddAggroFromDistance(Character target, float distance)
    {
        AddAggro(target, AggroFromDistance(distance));
    }

    public void SetAggro(Character target, float value)
    {
        if (target == null) return;
        _aggroTable[target] = Mathf.Max(0f, value);
    }

    /// <summary>가장 높은 어그로 대상. 없으면 null.</summary>
    public Character GetHighestAggroTarget()
    {
        Character best = null;
        float bestVal = 0f;
        foreach (var kv in _aggroTable)
        {
            if (kv.Key == null || kv.Key.Model == null || kv.Key.Model.IsDead) continue;
            if (kv.Value > bestVal)
            {
                bestVal = kv.Value;
                best = kv.Key;
            }
        }
        return best;
    }

    public bool HasAnyAboveThreshold()
    {
        foreach (var kv in _aggroTable)
        {
            if (kv.Key != null && !kv.Key.Model.IsDead && kv.Value >= AggroThreshold)
                return true;
        }
        return false;
    }

    /// <summary>리셋 후 비전투. true면 리셋됨.</summary>
    public bool TryResetIfOutOfRange(Vector3 myPos, Transform currentTarget)
    {
        if (currentTarget == null) return true;
        float dist = Vector3.Distance(myPos, currentTarget.position);
        if (dist > AggroLoseDistance)
        {
            ClearAll();
            return true;
        }
        return false;
    }

    public void ClearAll()
    {
        _aggroTable.Clear();
    }
}
