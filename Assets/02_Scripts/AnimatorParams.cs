/// <summary>
/// 애니메이터 파라미터 이름/해시를 한곳에 모아두기 (Enum처럼).
/// 플레이어 등에서 Animator.SetFloat / SetTrigger 시 이 값 사용.
/// </summary>
public static class AnimatorParams
{
    public const string MoveSpeedName = "MoveSpeed";
    public const string AttackName = "Attack";

    public static readonly int MoveSpeed = UnityEngine.Animator.StringToHash(MoveSpeedName);
    public static readonly int Attack = UnityEngine.Animator.StringToHash(AttackName);
}
