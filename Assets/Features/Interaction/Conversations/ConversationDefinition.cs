using System;

[Serializable]
public class ConversationDefinition
{
    public string Id;
    public string StartStepId;
    public ConversationStep[] Steps = new ConversationStep[3];
}