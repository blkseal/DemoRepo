using System;

[Serializable]
public class ConversationStep
{
    public string StepId;
    public string Question;
    public ConversationOption[] Options = new ConversationOption[2];
}