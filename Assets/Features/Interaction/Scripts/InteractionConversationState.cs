public class InteractionConversationState
{
    public ConversationSession Session { get; private set; }
    public bool HasActiveSession => Session != null && !Session.IsFinished;

    public void Start(ConversationDefinition definition)
    {
        Session = definition == null ? null : new ConversationSession(definition);
    }

    public void Clear()
    {
        Session = null;
    }
}