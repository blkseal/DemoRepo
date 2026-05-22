using UnityEngine;

public class Plate : MonoBehaviour
{
    public float dirtyTime = 10f;

    public GameObject foodModel;
    public GameObject emptyModel;

    bool isOnTable = false;

    public void OnPlacedOnTable()
    {
        isOnTable = true;
        Invoke(nameof(MakeDirty), dirtyTime);
    }

    public void OnPickedUp()
    {
        isOnTable = false;
        CancelInvoke(nameof(MakeDirty));
    }

    void MakeDirty()
    {
        if (!isOnTable) return;

        if (foodModel) foodModel.SetActive(false);
        if (emptyModel) emptyModel.SetActive(true);
    }
}