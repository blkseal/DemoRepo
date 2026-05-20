public static class ReviewCatalog
{
    public static readonly Review NegativeReview = new Review
    {
        Title = "Negative Review",
        Description = "Hell yes.",
        ReviewPoints = 1,
        SuspicionPointsMin = 1,
        SuspicionPointsMax = 3,
    };

    public static readonly Review PositiveReview = new Review
    {
        Title = "Positive Review",
        Description = "That's not gonna help...",
        ReviewPoints = -1,
        SuspicionPointsMin = -2,
        SuspicionPointsMax = -1,
    };

    public static readonly Review Complaint = new Review
    {
        Title = "Complaint",
        Description = "Good. Very nice.",
        ReviewPoints = 3,
        SuspicionPointsMin = 3,
        SuspicionPointsMax = 5,
    };

    public static readonly Review ManagerEscalation = new Review
    {
        Title = "I want to speak to your manager",
        Description = "Very lucky!",
        ReviewPoints = 10,
        SuspicionPointsMin = 5,
        SuspicionPointsMax = 10,
    };

    public static readonly Review PraiseBook = new Review
    {
        Title = "Praise Book Review",
        Description = "Oh... No...",
        ReviewPoints = -15,
        SuspicionPoints = -100,
    };

    public static Review Get(ReviewKind kind)
    {
        return kind switch
        {
            ReviewKind.NegativeReview => NegativeReview,
            ReviewKind.Complaint => Complaint,
            ReviewKind.ManagerEscalation => ManagerEscalation,
            ReviewKind.PositiveReview => PositiveReview,
            ReviewKind.PraiseBook => PraiseBook,
            _ => null,
        };
    }
}