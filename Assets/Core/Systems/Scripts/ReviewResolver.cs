using UnityEngine;

public static class ReviewResolver
{
    private const float KarenManagerMultiplier = 25f;
    private const float RichGuyComplaintMultiplier = 2f;
    private const float ChillGuyPraiseBookMultiplier = 10f;

    private struct WeightedReviewKind
    {
        public ReviewKind Kind;
        public float Weight;

        public WeightedReviewKind(ReviewKind kind, float weight)
        {
            Kind = kind;
            Weight = weight;
        }
    }

    public static Review ResolveReview(InteractionResult interactionResult, NpcType? npcType = null)
    {
        return ResolveReview(interactionResult, 0, npcType);
    }

    public static Review ResolveReview(InteractionResult interactionResult, int interactionStrength, NpcType? npcType = null)
    {
        var options = GetOptions(interactionResult, interactionStrength);
        if (options == null || options.Length == 0)
        {
            return null;
        }

        var totalWeight = 0f;
        for (var i = 0; i < options.Length; i++)
        {
            totalWeight += GetAdjustedWeight(options[i], npcType);
        }

        if (totalWeight <= 0f)
        {
            return null;
        }

        var roll = Random.value * totalWeight;
        for (var i = 0; i < options.Length; i++)
        {
            roll -= GetAdjustedWeight(options[i], npcType);
            if (roll <= 0f)
            {
                return ReviewCatalog.Get(options[i].Kind);
            }
        }

        return ReviewCatalog.Get(options[options.Length - 1].Kind);
    }

    private static WeightedReviewKind[] GetOptions(InteractionResult interactionResult, int interactionStrength)
    {
        return interactionResult switch
        {
            InteractionResult.Negative => GetNegativeOptions(interactionStrength),
            InteractionResult.Neutral => new[]
            {
                new WeightedReviewKind(ReviewKind.NegativeReview, 30f),
                new WeightedReviewKind(ReviewKind.PositiveReview, 30f),
                new WeightedReviewKind(ReviewKind.None, 40f),
            },
            InteractionResult.Positive => GetPositiveOptions(interactionStrength),
            _ => null,
        };
    }

    private static WeightedReviewKind[] GetNegativeOptions(int interactionStrength)
    {
        var extraNegative = Mathf.Max(0, -interactionStrength - 5);

        return new[]
        {
            new WeightedReviewKind(ReviewKind.NegativeReview, Mathf.Max(8f, 84f - (extraNegative * 6f))),
            new WeightedReviewKind(ReviewKind.Complaint, 14f + (extraNegative * 5f)),
            new WeightedReviewKind(ReviewKind.ManagerEscalation, 2f + (extraNegative * 1.5f)),
        };
    }

    private static WeightedReviewKind[] GetPositiveOptions(int interactionStrength)
    {
        var extraPositive = Mathf.Max(0, interactionStrength - 5);

        return new[]
        {
            new WeightedReviewKind(ReviewKind.PositiveReview, Mathf.Max(8f, 84f - (extraPositive * 6f))),
            new WeightedReviewKind(ReviewKind.Recommendation, 14f + (extraPositive * 5f)),
            new WeightedReviewKind(ReviewKind.PraiseBook, 2f + (extraPositive * 1.5f)),
        };
    }

    private static float GetAdjustedWeight(WeightedReviewKind reviewKind, NpcType? npcType)
    {
        var multiplier = 1f;

        if (npcType.HasValue)
        {
            multiplier = GetNpcTypeMultiplier(reviewKind.Kind, npcType.Value);
        }

        return reviewKind.Weight * multiplier;
    }

    private static float GetNpcTypeMultiplier(ReviewKind reviewKind, NpcType npcType)
    {
        return npcType switch
        {
            NpcType.Karen => reviewKind == ReviewKind.ManagerEscalation ? KarenManagerMultiplier : 1f,
            NpcType.RichGuy => reviewKind == ReviewKind.Complaint ? RichGuyComplaintMultiplier : 1f,
            NpcType.ChillGuy => reviewKind == ReviewKind.PraiseBook ? ChillGuyPraiseBookMultiplier : 1f,
            _ => 1f,
        };
    }
}