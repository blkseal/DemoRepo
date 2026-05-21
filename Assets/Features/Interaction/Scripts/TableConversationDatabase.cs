using UnityEngine;

public static class TableConversationDatabase
{
    // Placeholder for table-order conversations.
    // Add branching table conversations here later.

    public static readonly ConversationDefinition[] Conversations = { };

    public static ConversationDefinition GetRandomTableConversation(NpcType? npcType)
    {
        if (Conversations == null || Conversations.Length == 0)
        {
            return null;
        }

        return Conversations[Random.Range(0, Conversations.Length)];
    }
}