using UnityEngine;

public static class TakeAwayConversationDatabase
{
    // Take-away conversations support three player goals:
    // - get a bad review without always spiking suspicion
    // - keep some branches subtle and sarcastic
    // - reserve a few harder branches for stronger risk/reward moments

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
                        new ConversationOption { Label = "Sure, what's your name?", AnswerValue = 0, NextStepId = "generic_01_pickup" },
                        new ConversationOption { Label = "Only if you say please.", AnswerValue = -1, NextStepId = "generic_01_attitude" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_01_pickup",
                    Question = "It's under Jamie.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Here you go.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "You waited long enough.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_01_attitude",
                    Question = "Are you serious?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "No, just verifying the name.", AnswerValue = 0, NextStepId = "generic_01_pickup" },
                        new ConversationOption { Label = "A little, yes.", AnswerValue = -1, NextStepId = "end" },
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
                    Question = "Hi, can I place a new order?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Absolutely. What can I get started?", AnswerValue = 1, NextStepId = "generic_02_order" },
                        new ConversationOption { Label = "Depends how complicated it is.", AnswerValue = 0, NextStepId = "generic_02_order" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_02_order",
                    Question = "Just a burger and fries.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Easy enough.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Terrible choice.", AnswerValue = -1, NextStepId = "end" },
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
                    Question = "I think my drink was missing from the bag.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Sorry about that, we'll fix it.", AnswerValue = 1, NextStepId = "generic_03_followup" },
                        new ConversationOption { Label = "Maybe it escaped.", AnswerValue = -1, NextStepId = "generic_03_followup_2" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_03_followup",
                    Question = "Thanks. Can you add it now?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yes, right away.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "We can sort that out.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_03_followup_2",
                    Question = "Ha ha, very funny. Can you add it now?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Of course, sorry about that.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "You sure you didn't drink it?", AnswerValue = -1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Generic_04",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Can I change my fries to onion rings?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yeah, no problem.", AnswerValue = 1, NextStepId = "generic_04_followup" },
                        new ConversationOption { Label = "No swaps. Ever.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_04_followup",
                    Question = "Thanks. Also, make it extra crispy.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Absolutely.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "We can do that.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Generic_05",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Do you have anything vegetarian?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yes, a few options.", AnswerValue = 1, NextStepId = "generic_05_followup" },
                        new ConversationOption { Label = "Lettuce.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_05_followup",
                    Question = "Cool, what do you recommend?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "The veggie wrap.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Start eating meat.", AnswerValue = -4, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Generic_06",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "What time do you close?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We're open until 3 PM.", AnswerValue = 1, NextStepId = "generic_06_followup" },
                        new ConversationOption { Label = "At closing time.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_06_followup",
                    Question = "Ok. Can I place an order for 3 PM then?.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Of course.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Are you serious?", AnswerValue = -2, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Generic_07",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I need to change one item on my order.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Sure, what needs changing?", AnswerValue = 1, NextStepId = "generic_07_followup" },
                        new ConversationOption { Label = "We have a no return policy.", AnswerValue = -1, NextStepId = "generic_07_followup" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_07_followup",
                    Question = "I just need to change the sauce.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "No problem. We'll remake your order.", AnswerValue = 2, NextStepId = "end" },
                        new ConversationOption { Label = "And how exactly do you plan on doing that?", AnswerValue = -1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Generic_08",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "How long for two burgers and fries?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "About fifteen minutes.", AnswerValue = 1, NextStepId = "generic_08_followup" },
                        new ConversationOption { Label = "At least three business days.", AnswerValue = -2, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_08_followup",
                    Question = "Can you make sure the fries are fresh?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Absolutely.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Straight out of the fries factory.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Generic_09",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Hey, the food was great last time.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Thank you, we appreciate it.", AnswerValue = 2, NextStepId = "generic_09_followup" },
                        new ConversationOption { Label = "Why are you telling me this?", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_09_followup",
                    Question = "Seriously, I should leave a review!",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Glad to hear it.", AnswerValue = 2, NextStepId = "end" },
                        new ConversationOption { Label = "DO. NOT. LEAVE. A. REVIEW.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Generic_10",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Can I get ketchup packets with my bag?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Of course.", AnswerValue = 1, NextStepId = "generic_10_followup" },
                        new ConversationOption { Label = "Sure, just go to MacDonalds.", AnswerValue = -2, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_10_followup",
                    Question = "Thanks. Just enough for the fries.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Got it.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "You're gonna get some fries on your ketchup.", AnswerValue = -2, NextStepId = "end" },
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
                    Question = "Is my order going to be ready any time soon?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We're working on it now.", AnswerValue = -1, NextStepId = "karen_01_followup" },
                        new ConversationOption { Label = "No, and now you've made it worse.", AnswerValue = -2, NextStepId = "karen_01_followup" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_01_followup",
                    Question = "Unbelievable. I have places to be.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Then we'll hurry.", AnswerValue = -1, NextStepId = "end" },
                        new ConversationOption { Label = "You and the rest of the city.", AnswerValue = -3, NextStepId = "end" },
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
                        new ConversationOption { Label = "And I expect you to get out of my face.", AnswerValue = -3, NextStepId = "end" },
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
                        new ConversationOption { Label = "Pretty sure I've seen you on Youtube.", AnswerValue = 2, NextStepId = "karen_03_followup_2" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_03_followup",
                    Question = "I'm not paying for this attitude.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Then don't pay for the attitude.", AnswerValue = -1, NextStepId = "end" },
                        new ConversationOption { Label = "I sincerely apologize. Let's solve your issue.", AnswerValue = 3, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_03_followup_2",
                    Question = "Oh yeah? Where?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "One of those public freakout compilations.", AnswerValue = -7, NextStepId = "end" },
                        new ConversationOption { Label = "Some influencer clip, I think.", AnswerValue = 3, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Karen_04",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Why is there a delivery fee on my order?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Let me check that for you.", AnswerValue = 1, NextStepId = "karen_04_followup" },
                        new ConversationOption { Label = "Must have been a mistake.", AnswerValue = 0, NextStepId = "karen_04_followup" },
                    }
                },
                new ConversationStep
                {
                    StepId = "karen_04_followup",
                    Question = "It better be fixed before I call my lawyer.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'll sort it out.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Will your lawyer be ordering delivery?", AnswerValue = -3, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Karen_05",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I want to speak to the manager.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'll get them.", AnswerValue = -10, NextStepId = "end" },
                        new ConversationOption { Label = "You are speaking to the wrong person.", AnswerValue = -10, NextStepId = "karen_05_followup" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_05_followup",
                    Question = "I'm getting you fired if you don't get the manager right now.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Please, do.", AnswerValue = -10, NextStepId = "end" },
                        new ConversationOption { Label = "Let me get him.", AnswerValue = -10, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Karen_06",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "This is unacceptable customer service.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'm sorry to hear that.", AnswerValue = 0, NextStepId = "karen_06_followup" },
                        new ConversationOption { Label = "Then maybe leave.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_06_followup",
                    Question = "You should really do something about it.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We are.", AnswerValue = -1, NextStepId = "end" },
                        new ConversationOption { Label = "We'll try to help.", AnswerValue = 1, NextStepId = "end" },
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
                        new ConversationOption { Label = "The best costs extra.", AnswerValue = 1, NextStepId = "end" },
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
                        new ConversationOption { Label = "We'll take care of it.", AnswerValue = 2, NextStepId = "end" },
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
                        new ConversationOption { Label = "Then you've called the wrong place.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_03_followup",
                    Question = "Make it fast.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Understood.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "We'll prioritize it.", AnswerValue = 1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "RichGuy_04",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Do you have anything actually upscale?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yes, we do.", AnswerValue = 1, NextStepId = "rich_04_followup" },
                        new ConversationOption { Label = "What are you talking about?", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_04_followup",
                    Question = "Good. I don't have time for average.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Then we'll get it right.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "We don't do average.", AnswerValue = 1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "RichGuy_05",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Can you skip the queue for me?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'll see what I can do.", AnswerValue = 1, NextStepId = "rich_05_followup" },
                        new ConversationOption { Label = "Everyone waits.", AnswerValue = -2, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_05_followup",
                    Question = "Excellent. I expect priority treatment.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We'll prioritize it.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Only for our finest customers.", AnswerValue = 2, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "RichGuy_06",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Money isn't the issue. Quality is.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Then you've called the right place.", AnswerValue = 1, NextStepId = "rich_06_followup" },
                        new ConversationOption { Label = "Seems you're the issue, actually.", AnswerValue = -2, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_06_followup",
                    Question = "Good. I want the best version of everything.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We can make that happen.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "No problem.", AnswerValue = 1, NextStepId = "end" },
                    },
                },
            },
        },
    };

    public static readonly ConversationDefinition[] ChillGuyConversations = { };

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