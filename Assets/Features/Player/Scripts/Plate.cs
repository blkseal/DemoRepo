using UnityEngine;

public class Plate : MonoBehaviour
{
    public float timeToBecomeDirty = 10f;

    public GameObject foodModel;
    public GameObject emptyModel;

    bool isOnTable = false;

    public void OnPlacedOnTable()
    {
        isOnTable = true;
        Invoke(nameof(MakeDirty), timeToBecomeDirty);
    }

    public void OnPickedUp()
    {
        isOnTable = false;
        CancelInvoke(nameof(MakeDirty));
    }

    void MakeDirty()
    {
        if (!isOnTable) return;

        if (foodModel != null) foodModel.SetActive(false);
        if (emptyModel != null) emptyModel.SetActive(true);
    }
}