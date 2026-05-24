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

        new ConversationDefinition
        {
            Id = "Generic_03",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Hey, can I change my fries to onion rings?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yeah, no problem.", AnswerValue = 1, NextStepId = "generic_03_followup" },
                        new ConversationOption { Label = "No swaps. Ever.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_03_followup",
                    Question = "Thanks. Also, make it extra crispy.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Absolutely.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "That's the spirit.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
    };

    public static readonly ConversationDefinition[] KarenConversations =
    {
        new ConversationDefinition
        {
            Id = "Karen_01",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Is this order going to be ready any time soon?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We're working on it now.", AnswerValue = -2, NextStepId = "karen_01_followup" },
                        new ConversationOption { Label = "No, and now you've made it worse.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_01_followup",
                    Question = "Unbelievable. I have places to be.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Then we'll hurry.", AnswerValue = -3, NextStepId = "end" },
                        new ConversationOption { Label = "You and the rest of the city.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
            },
        },

        new ConversationDefinition
        {
            Id = "Karen_02",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I said no onions. There are onions.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'm sorry, we'll fix it.", AnswerValue = 1, NextStepId = "karen_02_followup" },
                        new ConversationOption { Label = "The onions have chosen their destiny.", AnswerValue = -2, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_02_followup",
                    Question = "I expect this remade properly.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Of course.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "We'll get it sorted.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },

        new ConversationDefinition
        {
            Id = "Karen_03",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Do you know who I am?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "A customer. How can I help?", AnswerValue = 0, NextStepId = "karen_03_followup" },
                        new ConversationOption { Label = "Someone who is already arguing.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_03_followup",
                    Question = "I'm not paying for this attitude.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Then don't pay for the attitude.", AnswerValue = -1, NextStepId = "end" },
                        new ConversationOption { Label = "Let's just fix the order.", AnswerValue = 1, NextStepId = "end" },
                    },
                },
            },
        },
    };

    public static readonly ConversationDefinition[] RichGuyConversations =
    {
        new ConversationDefinition
        {
            Id = "RichGuy_01",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I'm in a rush. Can you handle something premium?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Absolutely, sir.", AnswerValue = 1, NextStepId = "rich_01_followup" },
                        new ConversationOption { Label = "You can wait with the rest of us.", AnswerValue = -2, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_01_followup",
                    Question = "Good. I only want the best.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We can do that.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "The best costs extra.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },

        new ConversationDefinition
        {
            Id = "RichGuy_02",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Can you make sure my order is treated like a VIP?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Of course.", AnswerValue = 1, NextStepId = "rich_02_followup" },
                        new ConversationOption { Label = "VIP line starts after everyone else.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_02_followup",
                    Question = "Fine. Then make it flawless.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We'll take care of it.", AnswerValue = -3, NextStepId = "end" },
                        new ConversationOption { Label = "We'll do our best.", AnswerValue = 1, NextStepId = "end" },
                    },
                },
            },
        },

        new ConversationDefinition
        {
            Id = "RichGuy_03",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I want something expensive, not just good.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We can arrange that.", AnswerValue = 1, NextStepId = "rich_03_followup" },
                        new ConversationOption { Label = "Then you've called the wrong place.", AnswerValue = -2, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_03_followup",
                    Question = "Make it fast. My guests are waiting.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Understood.", AnswerValue = 0, NextStepId = "end" },
                        new ConversationOption { Label = "We'll prioritize it.", AnswerValue = 1, NextStepId = "end" },
                    },
                },
            },
        },
    };

    public static readonly ConversationDefinition[] ChillGuyConversations =
    {
        new ConversationDefinition
        {
            Id = "ChillGuy_01",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Yo, no stress, just checking on my order.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "You're good, it's on the way.", AnswerValue = 1, NextStepId = "chill_01_followup" },
                        new ConversationOption { Label = "Eventually. Maybe.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "chill_01_followup",
                    Question = "Nice. You guys still got that special?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yeah, absolutely.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "We've got a few things.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },

        new ConversationDefinition
        {
            Id = "ChillGuy_02",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Any chance I can tweak my order a little?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yeah, no problem.", AnswerValue = 1, NextStepId = "chill_02_followup" },
                        new ConversationOption { Label = "Only if the vibes allow it.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "chill_02_followup",
                    Question = "Sweet. Make it extra sauce, if possible.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Done.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Sauce is an art, but yes.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },

        new ConversationDefinition
        {
            Id = "ChillGuy_03",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Dude, take your time. I know it's busy.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Appreciate that.", AnswerValue = 1, NextStepId = "chill_03_followup" },
                        new ConversationOption { Label = "Finally, someone gets it.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "chill_03_followup",
                    Question = "No worries. I'm just hanging out anyway.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We'll get it to you soon.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "That's the right attitude.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
    };

    public static ConversationDefinition GetRandomTakeAwayConversation(NpcType? npcType)
    {
        var pool = GetPool(npcType);
        if (pool == null || pool.Length == 0)
        {
            pool = GenericConversations;
        }

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