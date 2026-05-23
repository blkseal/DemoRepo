using UnityEngine;
using System.Collections.Generic;

public static class TableConversationDatabase
{
    private static readonly string[] Dishes = { "Steak and Chips", "Sandwich", "Tuna Salad" };
    private static readonly string[] PositiveResponses = { "Right away.", "It will only take a minute!", "Good choice!" };
    private static readonly string[] NegativeResponses = { "Go grab it yourself.", "Oh hell no, that's disgusting.", "I hope you throw up." };

    private static readonly string[] IntroPositiveResponses = { "What will it be?", "Sure, what can I get you?", "Ready to take the order." };
    private static readonly string[] IntroNegativeResponses = { "Give me a minute.", "Hold on.", "Not right now." };

    public sealed class TableConversationData
    {
        public string IntroQuestion;
        public string IntroPositiveOption;
        public string IntroNegativeOption;
        public string OrderQuestion;
        public string PositiveOrderOption;
        public string NegativeOrderOption;
    }

    public static TableConversationData CreateConversation(int groupSize)
    {
        groupSize = Mathf.Max(1, groupSize);
        return new TableConversationData
        {
            IntroQuestion = "We're ready to order",
            IntroPositiveOption = IntroPositiveResponses[Random.Range(0, PositiveResponses.Length)],
            IntroNegativeOption = IntroNegativeResponses[Random.Range(0, NegativeResponses.Length)],
            OrderQuestion = BuildOrderText(groupSize),
            PositiveOrderOption = PositiveResponses[Random.Range(0, PositiveResponses.Length)],
            NegativeOrderOption = NegativeResponses[Random.Range(0, NegativeResponses.Length)],
        };
    }

    private static string BuildOrderText(int groupSize)
    {
        var orderCounts = new Dictionary<string, int>();
        for (var i = 0; i < groupSize; i++)
        {
            var dish = Dishes[Random.Range(0, Dishes.Length)];
            if (!orderCounts.ContainsKey(dish)) orderCounts[dish] = 0;
            orderCounts[dish]++;
        }

        var parts = new List<string>();
        foreach (var pair in orderCounts)
        {
            parts.Add($"{pair.Value} {FormatDishName(pair.Key, pair.Value)}");
        }

        return "We want " + string.Join(" and ", parts);
    }

    private static string FormatDishName(string dish, int count)
    {
        if (count <= 1) return dish;
        return dish switch
        {
            "Tuna Salad" => "Tuna Salads",
            "Sandwich" => "Sandwiches",
            _ => dish,
        };
    }
}