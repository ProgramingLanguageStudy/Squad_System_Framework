public interface IBrain
{
    void Initialize(Character character, CharacterStateMachine stateMachine);
    void Tick(); // 매 프레임 판단
}
