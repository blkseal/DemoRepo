using UnityEngine;

public enum PlateType
{
    MeatAndChips,
    TunaSalad,
    Sandwich,
    RottenFood
}

public class Plate : MonoBehaviour
{
    public PlateType plateType;

    [Tooltip("Seconds until the plate auto-empties if nobody eats it. Set high (e.g. 120) so it doesn't vanish while customers are eating.")]
    public float timeToEmpty = 120f;

    public GameObject foodModel;
    public GameObject emptyModel;

    bool isOnTable = false;
    bool isClaimed = false; // true when a customer is eating this plate

    /// <summary>Called by PlateSystem when the player places the plate on the table.</summary>
    public void OnPlacedOnTable()
    {
        if (isOnTable) return; // already on table – avoid double Invoke
        isOnTable = true;
        Invoke(nameof(MakeEmpty), timeToEmpty);
    }

    /// <summary>Called when a customer claims this plate (correct or wrong food).</summary>
    public void OnClaimed()
    {
        isClaimed = true;
        CancelInvoke(nameof(MakeEmpty)); // customers handle their own eating time
    }

    /// <summary>Called when the customer group finishes eating – visually empty the plate.</summary>
    public void OnEaten()
    {
        isClaimed = false;
        isOnTable = false;
        if (foodModel != null) foodModel.SetActive(false);
        if (emptyModel != null) emptyModel.SetActive(true);
    }

    public void OnPickedUp()
    {
        isOnTable = false;
        isClaimed = false;
        CancelInvoke(nameof(MakeEmpty));
    }

    void MakeEmpty()
    {
        if (!isOnTable || isClaimed) return;
        if (foodModel != null) foodModel.SetActive(false);
        if (emptyModel != null) emptyModel.SetActive(true);
    }
}