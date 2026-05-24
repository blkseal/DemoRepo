using UnityEngine;

public static class PhoneCallConversationDatabase
{
    public static readonly string[] GenericCallerNames =
    {
        "Alex",
        "Jamie",
        "Morgan",
        "Taylor",
        "Casey",
        "Sam",
    };

    public static readonly ConversationDefinition[] GenericConversations =
    {
        new ConversationDefinition
        {
            Id = "Phone_Generic_01",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Hello? I need to place an order.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Sure, what would you like?", AnswerValue = 1, NextStepId = "generic_01_followup" },
                        new ConversationOption { Label = "We are closed. Call later.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_01_followup",
                    Question = "Great, make that two burgers and fries.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Coming right up.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Yeah... We don't really do those.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Generic_02",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Hey, I ordered a Tuna Salad an hour ago...",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Sorry for the delay, we should be almost there.", AnswerValue = 0, NextStepId = "generic_02_followup" },
                        new ConversationOption { Label = "What do you want me to do about it?", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_02_followup",
                    Question = "Okay, but if it's cold I'm going to be mad.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Understood. We'll make it right.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Then eat it angrily, I guess.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Generic_03",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Hi, can you hold a table for tonight?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yes, I can do that.", AnswerValue = 1, NextStepId = "generic_03_followup" },
                        new ConversationOption { Label = "They're kinda heavy, I don't think I can.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_03_followup",
                    Question = "Perfect. Make it by the window.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'll note it down.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Sir, the tables have already been made.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Generic_04",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Hi, are you still delivering this late?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yep, we're still open.", AnswerValue = 1, NextStepId = "generic_04_followup" },
                        new ConversationOption { Label = "Only emotionally.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_04_followup",
                    Question = "Nice. I need it fast, then.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We'll send it over.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "If we're fast, we're gonna be furious.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Generic_05",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Can I get extra sauce with my order?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Of course.", AnswerValue = 1, NextStepId = "generic_05_followup" },
                        new ConversationOption { Label = "Sauce privileges revoked.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_05_followup",
                    Question = "Cool. Make it a lot of sauce.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "You got it.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "At this point it's a soup request.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Generic_06",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Hey, I think you forgot my drink.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Sorry about that, we'll fix it.", AnswerValue = 1, NextStepId = "generic_06_followup" },
                        new ConversationOption { Label = "Maybe it evaporated.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_06_followup",
                    Question = "Thanks. Please don't forget it twice.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We'll be careful.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "We most certainly will.", AnswerValue = -2, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Generic_07",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Do you have anything vegetarian?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yes, we have a few options.", AnswerValue = 1, NextStepId = "generic_07_followup" },
                        new ConversationOption { Label = "Lettuce.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_07_followup",
                    Question = "Nice. What would you recommend?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'd suggest the veggie wrap.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Stop being vegetarian.", AnswerValue = -2, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Generic_08",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Can I change my order after placing it?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'll see what I can do.", AnswerValue = 1, NextStepId = "generic_08_followup" },
                        new ConversationOption { Label = "Live with your decisions.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_08_followup",
                    Question = "Thanks. I just forgot one thing.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Tell me what to change.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "I'm sure you did.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Generic_09",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "How long for two burgers and fries?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "About fifteen minutes.", AnswerValue = 1, NextStepId = "generic_09_followup" },
                        new ConversationOption { Label = "At least three business days.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_09_followup",
                    Question = "Cool. Can you make sure the fries are fresh?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Absolutely.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Straight out of the tree.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Generic_10",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Hey, just wanted to say the food was great last time.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Thank you, we appreciate it!", AnswerValue = 1, NextStepId = "generic_10_followup" },
                        new ConversationOption { Label = "Oh. Ok...", AnswerValue = 1, NextStepId = "generic_10_followup" },
                    },
                },
                new ConversationStep
                {
                    StepId = "generic_10_followup",
                    Question = "Seriously, I was impressed.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Glad to hear it.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Please, for the love of god, do NOT leave a good review.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
    };

    public static readonly ConversationDefinition[] KarenConversations =
    {
        new ConversationDefinition
        {
            Id = "Phone_Karen_01",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I need 3 Tuna Salads. I'll be there in 10 minutes.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "See you soon.", AnswerValue = 0, NextStepId = "karen_01_followup" },
                        new ConversationOption { Label = "Ok. After you get here you'll have to wait a few more hours.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_01_followup",
                    Question = "Make it on time, or I'll leave.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We'll do our best.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Don't even bother coming.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Karen_02",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I'd like to place a delivery order.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We currently have a 30 minutes wait time.", AnswerValue = 0, NextStepId = "path_a" },
                        new ConversationOption { Label = "No.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "path_a",
                    Question = "That is too long, can't you do it faster?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Sure, I'll just teleport to you.", AnswerValue = -1, NextStepId = "path_a" },
                        new ConversationOption { Label = "No, but I could make it slower.", AnswerValue = -1, NextStepId = "karen_02_end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_02_end",
                    Question = "Unbelievable. I should speak to your manager.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Please do.", AnswerValue = -1, NextStepId = "end" },
                        new ConversationOption { Label = "Please don't.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition // Review from here
        {
            Id = "Phone_Karen_03",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I know the manager, can I have a discount?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Of course.", AnswerValue = 1, NextStepId = "karen_03_followup" },
                        new ConversationOption { Label = "Best I can do is charge extra.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_03_followup",
                    Question = "Good. They know me there.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'll make a note of it.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "And I know the menu.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Karen_04",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Your website says free delivery. Why am I being charged?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Let me check that for you.", AnswerValue = 1, NextStepId = "karen_04_followup" },
                        new ConversationOption { Label = "Because nothing in life is free.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_04_followup",
                    Question = "It better be fixed before I lose my patience.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'll sort it out.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Your patience is on backorder.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Karen_05",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I specifically said NO onions.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'm sorry, we'll remake it.", AnswerValue = 1, NextStepId = "karen_05_followup" },
                        new ConversationOption { Label = "The onions chose you.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_05_followup",
                    Question = "Fine. And make it fast.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We'll hurry.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Fast, onion-free, and haunted by your energy.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Karen_06",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I want to speak to whoever is in charge.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'll get the manager.", AnswerValue = 0, NextStepId = "karen_06_followup" },
                        new ConversationOption { Label = "That's me. Be afraid.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_06_followup",
                    Question = "Finally, someone sensible.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "One moment please.", AnswerValue = 0, NextStepId = "end" },
                        new ConversationOption { Label = "You're welcome.", AnswerValue = 1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_Karen_07",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "This is unacceptable customer service.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'm sorry to hear that.", AnswerValue = 0, NextStepId = "karen_07_followup" },
                        new ConversationOption { Label = "Yet somehow, you survived.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "karen_07_followup",
                    Question = "You should really do something about it.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We are.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "We're doing our best to stay calm.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
    };

    public static readonly ConversationDefinition[] RichGuyConversations =
    {
        new ConversationDefinition
        {
            Id = "Phone_RichGuy_01",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I'm looking for triple A filet mignon. Can you do that?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Absolutely, sir.", AnswerValue = 1, NextStepId = "rich_01_followup" },
                        new ConversationOption { Label = "Maybe try somewhere cheaper.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_01_followup",
                    Question = "Excellent. Make it flawless.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We will.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "No pressure at all.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_RichGuy_02",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I need the VIP version, and I need it now.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We can arrange that.", AnswerValue = 1, NextStepId = "rich_02_followup" },
                        new ConversationOption { Label = "You can wait in line.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_02_followup",
                    Question = "Good. I expect red carpet treatment.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Absolutely.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "We only have black carpet.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_RichGuy_03",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Can you add the luxury upgrade to my order?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Of course.", AnswerValue = 1, NextStepId = "rich_03_followup" },
                        new ConversationOption { Label = "Not worth the price.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_03_followup",
                    Question = "Perfect. I want the fancy version of everything.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'll make it happen.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "That sounds like a fun invoice.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_RichGuy_04",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I'd like your most expensive wine recommendation.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I have the perfect bottle for you.", AnswerValue = 1, NextStepId = "rich_04_followup" },
                        new ConversationOption { Label = "Wine all tastes the same anyway.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_04_followup",
                    Question = "Make sure it's impressive.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Absolutely.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Impressive is our middle name.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_RichGuy_05",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Can you make sure my order skips the queue?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'll prioritize it personally.", AnswerValue = 1, NextStepId = "rich_05_followup" },
                        new ConversationOption { Label = "Everyone waits their turn.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_05_followup",
                    Question = "Good. I don't like waiting.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Understood.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Nobody does, honestly.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_RichGuy_06",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Money isn't a problem. I just want the best.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Then you've called the right place.", AnswerValue = 1, NextStepId = "rich_06_followup" },
                        new ConversationOption { Label = "Good, because it'll cost you.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_06_followup",
                    Question = "Excellent. No compromises.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "No compromises.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Only tasteful ones.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_RichGuy_07",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I have guests arriving in twenty minutes. Don't disappoint me.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We'll make it happen.", AnswerValue = 1, NextStepId = "rich_07_followup" },
                        new ConversationOption { Label = "No promises.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_07_followup",
                    Question = "Good. They have expensive taste.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Then we're ready.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "So do we, apparently.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_RichGuy_08",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Can your chef prepare something off-menu for me?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I'll speak with the chef immediately.", AnswerValue = 1, NextStepId = "rich_08_followup" },
                        new ConversationOption { Label = "The menu exists for a reason.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_08_followup",
                    Question = "Perfect. Something exclusive would be ideal.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We'll handle it.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Exclusive is just expensive with attitude.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_RichGuy_09",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I'd like everything double portioned.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Certainly, sir.", AnswerValue = 1, NextStepId = "rich_09_followup" },
                        new ConversationOption { Label = "Trying to feed an army?", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_09_followup",
                    Question = "Good. Generosity matters.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "We'll make it generous.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "It's one way to say 'overflowing'.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_RichGuy_10",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I expect premium service for a premium customer.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "You have my full attention.", AnswerValue = 1, NextStepId = "rich_10_followup" },
                        new ConversationOption { Label = "You and everyone else.", AnswerValue = -1, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "rich_10_followup",
                    Question = "Excellent. That's what I like to hear.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Happy to help.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Premium attitude, premium inconvenience.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
    };

    public static readonly ConversationDefinition[] ChillGuyConversations =
    {
        new ConversationDefinition
        {
            Id = "Phone_ChillGuy_01",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Yo, what is good? I need a quick favor.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yeah, what do you need?", AnswerValue = 1, NextStepId = "chill_01_followup" },
                        new ConversationOption { Label = "Not right now.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "chill_01_followup",
                    Question = "Could you just keep an eye on it for me?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Sure thing.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "No worries, I got you.", AnswerValue = 1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_ChillGuy_02",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Any chance you can help me out real quick?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Sure, shoot.", AnswerValue = 1, NextStepId = "chill_02_followup" },
                        new ConversationOption { Label = "Maybe later.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "chill_02_followup",
                    Question = "Awesome. No rush, just when you can.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Sounds good.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "That is very reasonable of you.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_ChillGuy_03",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "I'm just checking in. Everything cool?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "All good here.", AnswerValue = 1, NextStepId = "chill_03_followup" },
                        new ConversationOption { Label = "Busy right now.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "chill_03_followup",
                    Question = "Nice, nice. Keep doing your thing.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Will do.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Appreciate the vibe.", AnswerValue = 1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_ChillGuy_04",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Hey man, no rush. Just wondering if my order is close.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yeah, it should be there soon.", AnswerValue = 1, NextStepId = "chill_04_followup" },
                        new ConversationOption { Label = "Eventually. Probably.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "chill_04_followup",
                    Question = "Sweet. I'm just hanging out anyway.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Sounds chill.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "That is the best way to wait.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_ChillGuy_05",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "You guys got any recommendations today?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Yeah, our specials are really good.", AnswerValue = 1, NextStepId = "chill_05_followup" },
                        new ConversationOption { Label = "Water.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "chill_05_followup",
                    Question = "Nice. Hit me with the good stuff then.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "I got you.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "Our chef would approve.", AnswerValue = 1, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_ChillGuy_06",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Take your time, dude. I know it gets busy.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Thanks for understanding.", AnswerValue = 1, NextStepId = "chill_06_followup" },
                        new ConversationOption { Label = "Finally, a reasonable human.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "chill_06_followup",
                    Question = "No stress. I'll be here.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Appreciate it.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "You're making waiting look easy.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
            },
        },
        new ConversationDefinition
        {
            Id = "Phone_ChillGuy_07",
            StartStepId = "start",
            Steps = new[]
            {
                new ConversationStep
                {
                    StepId = "start",
                    Question = "Can I get fries with literally everything?",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Absolutely.", AnswerValue = 1, NextStepId = "chill_07_followup" },
                        new ConversationOption { Label = "That's medically concerning.", AnswerValue = 0, NextStepId = "end" },
                    },
                },
                new ConversationStep
                {
                    StepId = "chill_07_followup",
                    Question = "Legend. You're speaking my language.",
                    Options = new[]
                    {
                        new ConversationOption { Label = "Fries are universal.", AnswerValue = 1, NextStepId = "end" },
                        new ConversationOption { Label = "A man of culture.", AnswerValue = 1, NextStepId = "end" },
                    },
                },
            },
        },
    };

    public static ConversationDefinition GetRandomPhoneCallConversation(NpcType? npcType)
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

    public static string GetRandomCallerName()
    {
        if (GenericCallerNames == null || GenericCallerNames.Length == 0)
        {
            return "Unknown Caller";
        }

        return GenericCallerNames[Random.Range(0, GenericCallerNames.Length)];
    }

    public static string GetCallerDisplayName(NpcType? npcType, string fallbackName)
    {
        return npcType.HasValue ? GetNpcTypeDisplayName(npcType.Value) : fallbackName;
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

    private static string GetNpcTypeDisplayName(NpcType npcType)
    {
        return npcType switch
        {
            NpcType.Karen => "Karen",
            NpcType.RichGuy => "Rich Guy",
            NpcType.ChillGuy => "Chill Guy",
            _ => npcType.ToString(),
        };
    }
}