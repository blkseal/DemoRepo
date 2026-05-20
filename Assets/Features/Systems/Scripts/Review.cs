using System;
using UnityEngine;

[Serializable]
public class Review
{
    public string Title;
    public string Description;
    public int ReviewPoints;
    public int SuspicionPoints;
    public int SuspicionPointsMin;
    public int SuspicionPointsMax;

    public int GetSuspicionPoints()
    {
        if (SuspicionPointsMin == 0 && SuspicionPointsMax == 0)
        {
            return SuspicionPoints;
        }

        var min = Mathf.Min(SuspicionPointsMin, SuspicionPointsMax);
        var max = Mathf.Max(SuspicionPointsMin, SuspicionPointsMax);
        return Random.Range(min, max + 1);
    }
}