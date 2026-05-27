using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages a group of seated NPCs: ordering, waiting for food, food validation, and leaving.
///
/// FLOW:
///  1. Spawn  -> SetTableZone() links this group to the physical TableZone.
///  2. Player talks -> order is taken, GenerateOrder() fills pendingOrder list.
///  3. Wait timer starts (maxWaitTime seconds, default 30).
///  4. Player places a plate on the linked TableZone -> NotifyPlatePlaced() fires.
///  5. HandlePlatePlaced:
///       Correct  -> removes from pending list, adds bonus wait time, plate.OnClaimed()
///       Incorrect-> NPCs eat anyway, suspicion/review malus applied, plate.OnClaimed()
///  6. All orders served -> group eats for eatTime seconds, plates become empty, group leaves.
///  7. Timer hits 0 -> group leaves angry (big suspicion hit), unclaimed plates auto-empty.
/// </summary>
public class NpcGroupInteractable : MonoBehaviour, IConversationTarget
{
    // ------------------------------------------------------------------ config
    [SerializeField] private bool hasNpcType;
    [SerializeField] private NpcType npcType;

    [Header("Wait Timer")]
    [Tooltip("Seconds the group waits before leaving if not fully served.")]
    [SerializeField] private float maxWaitTime = 30f;
    [Tooltip("Extra seconds added to the timer each time a plate is served.")]
    [SerializeField] private float bonusTimePerServedPlate = 20f;
    [Tooltip("Seconds the group stays eating after all plates delivered.")]
    [SerializeField] private float eatTime = 15f;

    [Header("Wrong food penalty")]
    [SerializeField] private int wrongFoodSuspicionPenalty = 5;
    [SerializeField] private int wrongFoodReviewPenalty = -10;

    [Header("Leave angry penalty")]
    [SerializeField] private int leaveAngrySuspicionPenalty = 15;
    [SerializeField] private int leaveAngryReviewPenalty = -20;

    // ----------------------------------------------------------------- runtime
    private NpcInteractable[] members;
    private TableConversationDatabase.TableConversationData conversationData;
    private bool awaitingOrder;
    private bool orderCompleted;

    // Dialog text (IConversationTarget)
    private string questionText  = "We're ready to order.";
    private string optionOneText = "What will it be?";
    private string optionTwoText = "Give me a minute.";

    // Order & serving
    private readonly List<PlateType> pendingOrder = new List<PlateType>();
    private readonly List<Plate>     claimedPlates = new List<Plate>();
    private TableZone associatedTableZone;

    // Timer
    private bool  isWaitingForFood;
    private float waitTimer;

    // -------------------------------------------------------- IConversationTarget
    public bool     CanInteract           => true;
    public bool     ApplyStatusEffects    => true;
    public bool     UseReviewResolver     => false;
    public NpcType? CustomerNpcType       => hasNpcType ? npcType : (NpcType?)null;
    public string   ConversationTitleText => string.Empty;
    public string   QuestionText          => questionText;
    public string   OptionOneText         => optionOneText;
    public string   OptionTwoText         => optionTwoText;

    // ================================================================ Public API
    public void SetMembers(NpcInteractable[] groupMembers)
    {
        members = groupMembers;
    }

    /// <summary>Links this group to the physical TableZone for plate-placement events.</summary>
    public void SetTableZone(TableZone tableZone)
    {
        associatedTableZone = tableZone;
        if (associatedTableZone != null)
            associatedTableZone.OnPlatePlaced += HandlePlatePlaced;
    }

    // ================================================================ Unity
    private void Update()
    {
        if (!isWaitingForFood) return;

        waitTimer -= Time.deltaTime;
        if (waitTimer <= 0f)
            LeaveAngry();
    }

    private void OnDestroy()
    {
        if (associatedTableZone != null)
            associatedTableZone.OnPlatePlaced -= HandlePlatePlaced;
    }

    // ================================================================ Dialog
    public void Interact()
    {
        if (orderCompleted)
        {
            questionText  = "We've already ordered.";
            optionOneText = "Ok!";
            optionTwoText = string.Empty;
        }
        else
        {
            awaitingOrder    = false;

            // Generate order now so the dialog knows what to display
            if (pendingOrder.Count == 0)
                GenerateOrder();

            conversationData = TableConversationDatabase.CreateConversation(pendingOrder);

            questionText  = conversationData.IntroQuestion;
            optionOneText = conversationData.IntroPositiveOption;
            optionTwoText = conversationData.IntroNegativeOption;
        }

        Debug.Log($"[Group] Interacted – {(members != null ? members.Length : 0)} member(s).");
        PlayerInteractionState.SetLocked(true);
        NpcInteractionUI.Instance.Show(this);
    }

    public InteractionOutcome ResolveAnswer(int answerIndex)
    {
        if (orderCompleted)
            return Neutral(finished: true);

        if (conversationData == null)
            return Neutral(finished: true);

        // Step 1 – "We're ready to order"
        if (!awaitingOrder)
        {
            if (answerIndex == 0) // "What will it be?"
            {
                awaitingOrder = true;
                questionText  = conversationData.OrderQuestion;
                optionOneText = conversationData.PositiveOrderOption;
                optionTwoText = conversationData.NegativeOrderOption;
                return Neutral(finished: false);
            }

            // Refused to take order
            return new InteractionOutcome
            {
                ConversationFinished = true,
                InteractionResult    = InteractionResult.Negative,
                SuspicionDelta       = 2,
                ReviewPointsDelta    = 0,
            };
        }

        // Step 2 – Order confirmed
        orderCompleted = true;
        bool isPositive = (answerIndex == 0);

        BeginWaiting();

        return new InteractionOutcome
        {
            ConversationFinished = true,
            InteractionResult    = isPositive ? InteractionResult.Positive : InteractionResult.Negative,
            SuspicionDelta       = isPositive ? 0 : 2,
            ReviewPointsDelta    = 0,
        };
    }

    // ================================================================ Order
    private void GenerateOrder()
    {
        pendingOrder.Clear();
        int count = members != null ? members.Length : 1;

        for (int i = 0; i < count; i++)
        {
            PlateType ordered = (PlateType)Random.Range(0, 3); // 0-2: no RottenFood
            pendingOrder.Add(ordered);
            Debug.Log($"[Group] Customer {i} ordered: {ordered}");
        }
    }

    private void BeginWaiting()
    {
        isWaitingForFood = true;
        waitTimer        = maxWaitTime;
        Debug.Log($"[Group] Waiting for {pendingOrder.Count} plate(s). Timer: {maxWaitTime}s");
    }

    // ================================================================ Plate validation
    /// <summary>Called by TableZone whenever the player places a plate on this table.</summary>
    private void HandlePlatePlaced(Plate plate)
    {
        if (!isWaitingForFood || plate == null)   return;
        if (claimedPlates.Contains(plate))         return; // already processed

        // Mark plate as being eaten — cancels its auto-empty timer
        plate.OnClaimed();
        claimedPlates.Add(plate);

        // Extend wait timer for every plate served
        waitTimer += bonusTimePerServedPlate;

        // Check if this plate type matches a pending order
        bool matched = false;
        for (int i = 0; i < pendingOrder.Count; i++)
        {
            if (pendingOrder[i] == plate.plateType)
            {
                pendingOrder.RemoveAt(i);
                matched = true;
                break;
            }
        }

        if (matched)
        {
            Debug.Log($"[Group] ✅ Correct plate ({plate.plateType})! Timer now {waitTimer:F1}s. Still waiting: {pendingOrder.Count}");
        }
        else
        {
            Debug.Log($"[Group] ❌ Wrong plate ({plate.plateType})! Complaint filed.");
            if (GameStatusSystem.Instance != null)
                GameStatusSystem.Instance.ApplyInteractionResult(wrongFoodSuspicionPenalty, wrongFoodReviewPenalty);
        }

        // All pending orders fulfilled?
        if (pendingOrder.Count == 0)
            AllServed();
    }

    // ================================================================ Leave
    private void AllServed()
    {
        isWaitingForFood = false;
        Debug.Log($"[Group] ✅ All served! Eating for {eatTime}s then leaving.");
        Invoke(nameof(LeaveSatisfied), eatTime);
    }

    private void LeaveAngry()
    {
        isWaitingForFood = false;
        CancelInvoke(nameof(LeaveSatisfied));
        Debug.Log("[Group] ⏰ Timer expired – customers leaving angry!");

        if (GameStatusSystem.Instance != null)
            GameStatusSystem.Instance.ApplyInteractionResult(leaveAngrySuspicionPenalty, leaveAngryReviewPenalty);

        EmptyClaimedPlates();
        DespawnAll();
    }

    private void LeaveSatisfied()
    {
        Debug.Log("[Group] 😊 Leaving satisfied.");
        EmptyClaimedPlates();
        DespawnAll();
    }

    private void EmptyClaimedPlates()
    {
        foreach (var plate in claimedPlates)
            if (plate != null) plate.OnEaten();
    }

    private void DespawnAll()
    {
        if (members != null)
            foreach (var m in members)
                if (m != null) m.StandUpAndLeave(); // walk to exit, then auto-despawn via trigger

        // Destroy the group root object after a delay to give NPCs time to start moving
        Destroy(gameObject, 1f);
    }

    // ================================================================ Helpers
    private static InteractionOutcome Neutral(bool finished) => new InteractionOutcome
    {
        ConversationFinished = finished,
        InteractionResult    = InteractionResult.Neutral,
        SuspicionDelta       = 0,
        ReviewPointsDelta    = 0,
    };
}