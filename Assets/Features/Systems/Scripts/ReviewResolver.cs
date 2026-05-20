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
        var options = GetOptions(interactionResult);
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

    private static WeightedReviewKind[] GetOptions(InteractionResult interactionResult)
    {
        return interactionResult switch
        {
            InteractionResult.Negative => new[]
            {
                new WeightedReviewKind(ReviewKind.NegativeReview, 78f),
                new WeightedReviewKind(ReviewKind.Complaint, 20f),
                new WeightedReviewKind(ReviewKind.ManagerEscalation, 2f),
            },
            InteractionResult.Neutral => new[]
            {
                new WeightedReviewKind(ReviewKind.NegativeReview, 30f),
                new WeightedReviewKind(ReviewKind.PositiveReview, 30f),
                new WeightedReviewKind(ReviewKind.None, 40f),
            },
            InteractionResult.Positive => new[]
            {
                new WeightedReviewKind(ReviewKind.PositiveReview, 98f),
                new WeightedReviewKind(ReviewKind.PraiseBook, 2f),
            },
            _ => null,
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