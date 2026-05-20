using UnityEngine;

public class GameStatusSystem : MonoBehaviour
{
    [Header("Player Status")]
    [Range(0, 100)]
    [SerializeField] private int suspicionLevel;

    [Range(-100, 100)]
    [SerializeField] private int reviewPoints;

    public int SuspicionLevel => suspicionLevel;
    public int ReviewPoints => reviewPoints;

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

    public void ApplyInteractionResult(int suspicionDelta, int reviewPointsDelta)
    {
        CalculateInteractionSuspicion(suspicionDelta);
        CalculateInteractionReviewPoints(reviewPointsDelta);
    }
}