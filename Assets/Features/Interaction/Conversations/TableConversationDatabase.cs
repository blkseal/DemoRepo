using UnityEngine;
using System.Collections.Generic;

public static class TableConversationDatabase
{
    private static readonly string[] Dishes = { "Meat and Chips", "Tuna Salad", "Sandwich", "Rotten Food" };
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

    public static TableConversationData CreateConversation(List<PlateType> actualOrder)
    {
        return new TableConversationData
        {
            IntroQuestion = "We're ready to order",
            IntroPositiveOption = IntroPositiveResponses[Random.Range(0, PositiveResponses.Length)],
            IntroNegativeOption = IntroNegativeResponses[Random.Range(0, NegativeResponses.Length)],
            OrderQuestion = BuildOrderText(actualOrder),
            PositiveOrderOption = PositiveResponses[Random.Range(0, PositiveResponses.Length)],
            NegativeOrderOption = NegativeResponses[Random.Range(0, NegativeResponses.Length)],
        };
    }

    private static string BuildOrderText(List<PlateType> order)
    {
        var orderCounts = new Dictionary<string, int>();
        foreach (var type in order)
        {
            string dishName = Dishes[(int)type];
            if (!orderCounts.ContainsKey(dishName)) orderCounts[dishName] = 0;
            orderCounts[dishName]++;
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
            "Rotten Food" => "Rotten Foods",
            _ => dish,
        };
    }
}