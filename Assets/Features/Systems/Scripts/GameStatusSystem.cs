using System;
using UnityEngine;

public class GameStatusSystem : MonoBehaviour
{
    private static GameStatusSystem instance;

    [Header("Player Status")]
    [Range(0, 100)]
    [SerializeField] private int suspicionLevel;

    [Range(-100, 100)]
    [SerializeField] private int reviewPoints;

    public static GameStatusSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = UnityEngine.Object.FindFirstObjectByType<GameStatusSystem>();
            }

            return instance;
        }
    }

    public event Action<int, int, Review> StatusChanged;

    public int SuspicionLevel => suspicionLevel;
    public int ReviewPoints => reviewPoints;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        GameStatusHUD.EnsureInstance();
        NotifyStatusChanged(null);
    }

    public int CalculateInteractionSuspicion(int suspicionDelta)
    {
        suspicionLevel = Mathf.Clamp(suspicionLevel + suspicionDelta, 0, 100);
        return suspicionLevel;
    }

    public int CalculateInteractionReviewPoints(int reviewPointsDelta)
    {
        reviewPoints = Mathf.Clamp(reviewPoints + reviewPointsDelta, -100, 100);
        return reviewPoints;
    }

    public Review ApplyInteractionResult(InteractionResult interactionResult, NpcType? npcType = null)
    {
        var review = ReviewResolver.ResolveReview(interactionResult, npcType);
        if (review == null)
        {
            NotifyStatusChanged(null);
            return null;
        }

        CalculateInteractionSuspicion(review.GetSuspicionPoints());
        CalculateInteractionReviewPoints(review.ReviewPoints);
        NotifyStatusChanged(review);
        return review;
    }

    public void ApplyInteractionResult(int suspicionDelta, int reviewPointsDelta)
    {
        CalculateInteractionSuspicion(suspicionDelta);
        CalculateInteractionReviewPoints(reviewPointsDelta);
        NotifyStatusChanged(null);
    }

    private void NotifyStatusChanged(Review review)
    {
        StatusChanged?.Invoke(suspicionLevel, reviewPoints, review);
    }
}