public interface IConversationTarget
{
    bool CanInteract { get; }
    bool ApplyStatusEffects { get; }
    NpcType? CustomerNpcType { get; }
    string QuestionText { get; }
    string OptionOneText { get; }
    string OptionTwoText { get; }
    void Interact();
    InteractionOutcome ResolveAnswer(int answerIndex);
}