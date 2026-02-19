using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 분대 제어. CharacterData 기반으로 동료 스폰·플레이어 따라가기 설정.
/// 씬에 하나 배치 후 인스펙터에서 followTarget·초기 동료 데이터 할당.
/// </summary>
public class SquadController : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] [Tooltip("따라갈 대상(플레이어 캐릭터)")]
    private Transform _followTarget;
    [SerializeField] [Tooltip("스폰된 동료 부모(비어 있으면 this)")]
    private Transform _squadRoot;

    [Header("초기 분대")]
    [SerializeField] [Tooltip("스폰할 캐릭터 데이터 목록. prefab은 각 Data에 있어야 함")]
    private List<CharacterData> _initialSquad = new List<CharacterData>();

    [Header("스폰")]
    [SerializeField] [Tooltip("대상 기준 스폰 반경. NavMesh 샘플링에 사용")]
    private float _spawnRadius = 2f;

    private readonly List<Character> _characters = new List<Character>();

    public IReadOnlyList<Character> Characters => _characters;

    /// <summary>따라갈 대상 설정. PlayScene 등에서 플레이어 Character.transform 전달. 대상 본인은 제외.</summary>
    public void SetFollowTarget(Transform target)
    {
        _followTarget = target;
        foreach (var c in _characters)
        {
            if (c == null) continue;
            if (c.transform == target) continue; // 자기 자신은 제외
            c.SetFollowTarget(target);
        }
    }

    public void Initialize()
    {
        SpawnInitialSquad();
    }

    private void SpawnInitialSquad()
    {
        var root = _squadRoot != null ? _squadRoot : transform;
        var basePos = _followTarget != null ? _followTarget.position : transform.position;
        int index = 0;

        foreach (var data in _initialSquad)
        {
            if (data == null || data.prefab == null)
            {
                Debug.LogWarning("[SquadController] CharacterData 또는 prefab이 비어 있습니다. 스킵합니다.");
                continue;
            }

            var offset = GetSpawnOffset(index);
            SpawnCharacter(data, basePos + offset, root);
            index++;
        }
    }

    private Vector3 GetSpawnOffset(int index)
    {
        if (index <= 0) return Vector3.zero;
        float angle = (360f / 4f) * index * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle) * _spawnRadius, 0f, Mathf.Sin(angle) * _spawnRadius);
    }

    /// <summary>캐릭터 1명 스폰. NavMesh 유효 위치에 배치 후 follow target 설정.</summary>
    public Character SpawnCharacter(CharacterData data, Vector3 nearPosition, Transform parent = null)
    {
        if (data == null || data.prefab == null) return null;

        if (!NavMesh.SamplePosition(nearPosition, out var hit, _spawnRadius * 2f, NavMesh.AllAreas))
        {
            Debug.LogWarning($"[SquadController] NavMesh 샘플 실패. nearPosition={nearPosition}");
            hit.position = nearPosition;
        }

        var root = parent != null ? parent : transform;
        var instance = Instantiate(data.prefab, hit.position, Quaternion.identity, root);
        var character = instance.GetComponent<Character>();

        if (character == null)
        {
            Debug.LogWarning("[SquadController] 프리팹에 Character 컴포넌트가 없습니다.");
            return null;
        }

        var model = character.Model;
        if (model != null && model.Data != data)
            model.Initialize(data);

        character.Initialize();
        character.SetAsCompanion(_followTarget);
        _characters.Add(character);
        return character;
    }

    public void RemoveCharacter(Character character)
    {
        if (character == null) return;
        _characters.Remove(character);
        character.SetFollowTarget(null);
        Destroy(character.gameObject);
    }
}
