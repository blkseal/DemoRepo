public interface IConversationTarget
{
    bool CanInteract { get; }
    bool ApplyStatusEffects { get; }
    bool UseReviewResolver { get; }
    NpcType? CustomerNpcType { get; }
    string ConversationTitleText { get; }
    string QuestionText { get; }
    string OptionOneText { get; }
    string OptionTwoText { get; }
    void Interact();
    InteractionOutcome ResolveAnswer(int answerIndex);
}