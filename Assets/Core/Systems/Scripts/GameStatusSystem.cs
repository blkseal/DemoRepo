using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        DontDestroyOnLoad(gameObject);
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
        return ApplyInteractionResult(interactionResult, 0, npcType);
    }

    public Review ApplyInteractionResult(InteractionResult interactionResult, int interactionStrength, NpcType? npcType = null)
    {
        var review = ReviewResolver.ResolveReview(interactionResult, interactionStrength, npcType);
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

    // Reset the internal status values and notify listeners
    public void ResetStatus(int initialSuspicion = 0, int initialReview = 0)
    {
        suspicionLevel = Mathf.Clamp(initialSuspicion, 0, 100);
        reviewPoints = Mathf.Clamp(initialReview, -100, 100);
        NotifyStatusChanged(null);
    }

    private void NotifyStatusChanged(Review review)
    {
        StatusChanged?.Invoke(suspicionLevel, reviewPoints, review);

        // If suspicion has reached the max (100), trigger game over via GameManager if present
        if (suspicionLevel >= 100)
        {
            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.GameOver();
            }
            else
            {
                // Fallback: save final review and load game over scene directly
                PlayerPrefs.SetInt("FinalReview", reviewPoints);
                PlayerPrefs.Save();
                SceneManager.LoadScene("GameOverScene");
            }
        }
    }
}