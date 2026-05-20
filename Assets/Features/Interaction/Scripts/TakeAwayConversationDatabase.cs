using UnityEngine;

public static class TakeAwayConversationDatabase
{
    // Add conversations here by customer type.
    // Each conversation is a branching tree:
    // - each step has a StepId
    // - each option can point to a NextStepId
    // - use a null/empty NextStepId to end the conversation

    public static readonly ConversationDefinition[] GenericConversations =
    {
        // Example template:
         new ConversationDefinition
         {
             Id = "Generic_01",
             StartStepId = "start",
             Steps = new[]
             {
                 new ConversationStep
                 {
                     StepId = "start",
                     Question = "Can I get my order?",
                     Options = new[]
                     {
                         new ConversationOption { Label = "Sure! What's your name?", AnswerValue = 0, NextStepId = "path_a" },
                         new ConversationOption { Label = "Only if you say please.", AnswerValue = 0, NextStepId = "path_b" },
                     },
                 },
                 new ConversationStep
                 {
                     StepId = "path_a",
                     Question = "I'm John Pork.",
                     Options = new[]
                     {
                         new ConversationOption { Label = "I have your order right here sir.", AnswerValue = 0, NextStepId = "end" },
                         new ConversationOption { Label = "What a beautiful name. Here you go good sir.", AnswerValue = 1, NextStepId = "end" },
                     },
                 },
                 new ConversationStep
                 {
                     StepId = "path_b",
                     Question = "Are you serious?",
                     Options = new[]
                     {
                         new ConversationOption { Label = "No. What's your name?", AnswerValue = 0, NextStepId = "path_a" },
                         new ConversationOption { Label = "Well now you're gonna have to beg.", AnswerValue = -1, NextStepId = "path_b_1" },
                     },
                 },
                   new ConversationStep
                 {
                     StepId = "path_b_1",
                     Question = "Do you know who I am? I'm John Pork.",
                     Options = new[]
                     {
                         new ConversationOption { Label = "No. And I don't care.", AnswerValue = -1, NextStepId = "end" },
                         new ConversationOption { Label = "Sorry. Here's your order.", AnswerValue = 1, NextStepId = "end" },
                     },
                 },
             },
         },

         new ConversationDefinition
         {
             Id = "Generic_02",
             StartStepId = "start",
             Steps = new[]
             {
                 new ConversationStep
                 {
                     StepId = "start",
                     Question = "Hello! Can I get some spicy chicken?",
                     Options = new[]
                     {
                         new ConversationOption { Label = "Sure! Right away.", AnswerValue = 1, NextStepId = "end" },
                         new ConversationOption { Label = "Come cook it yourself.", AnswerValue = 0, NextStepId = "path_a" },
                     },
                 },
                 new ConversationStep
                 {
                     StepId = "path_a",
                     Question = "What's wrong with you?",
                     Options = new[]
                     {
                         new ConversationOption { Label = "I mean, I cooked it myself! Here you go sir.", AnswerValue = 0, NextStepId = "end" },
                         new ConversationOption { Label = "Either get in here and cook it yourself, or leave.", AnswerValue = -1, NextStepId = "end" },
                     },
                 },
             },
         },




    };

    public static readonly ConversationDefinition[] KarenConversations = { };
    public static readonly ConversationDefinition[] RichGuyConversations = { };
    public static readonly ConversationDefinition[] ChillGuyConversations = { };

    public static ConversationDefinition GetRandomTakeAwayConversation(NpcType? npcType)
    {
        var pool = GetPool(npcType);
        if (pool == null || pool.Length == 0)
        {
            return null;
        }

        return pool[Random.Range(0, pool.Length)];
    }

    private static ConversationDefinition[] GetPool(NpcType? npcType)
    {
        return npcType switch
        {
            NpcType.Karen => KarenConversations,
            NpcType.RichGuy => RichGuyConversations,
            NpcType.ChillGuy => ChillGuyConversations,
            _ => GenericConversations,
        };
    }
}