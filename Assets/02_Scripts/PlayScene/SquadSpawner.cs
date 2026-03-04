using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 분대 캐릭터 스폰·삭제 담당. SquadController 하위 컴포넌트.
/// </summary>
public class SquadSpawner : MonoBehaviour
{
    /// <summary>캐릭터 1명 스폰. NavMesh 유효 위치에 배치. Player/Companion 설정은 호출부에서.</summary>
    /// <param name="navSampleRadius">NavMesh 샘플링 반경. SquadController._spawnRadius와 동일 권장.</param>
    public Character Spawn(CharacterData data, Vector3 nearPosition, Transform parent, CombatController combatController, float navSampleRadius = 2f)
    {
        if (data == null || data.prefab == null) return null;

        if (!NavMesh.SamplePosition(nearPosition, out var hit, navSampleRadius * 2f, NavMesh.AllAreas))
        {
            Debug.LogWarning($"[SquadSpawner] NavMesh 샘플 실패. nearPosition={nearPosition}");
            hit.position = nearPosition;
        }

        var instance = Instantiate(data.prefab, hit.position, Quaternion.identity, parent);
        var character = instance.GetComponent<Character>();

        if (character == null)
        {
            Debug.LogWarning("[SquadSpawner] 프리팹에 Character 컴포넌트가 없습니다.");
            Destroy(instance);
            return null;
        }

        var model = character.Model;
        if (model != null && model.Data != data)
            model.Initialize(data);

        character.Initialize(combatController);
        return character;
    }

    /// <summary>캐릭터 제거. GameObject 파괴.</summary>
    public void DestroyCharacter(Character character)
    {
        if (character == null) return;
        character.SetFollowTarget(null);
        Destroy(character.gameObject);
    }
}
