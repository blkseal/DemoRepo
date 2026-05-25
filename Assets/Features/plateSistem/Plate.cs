using UnityEngine;

public class Plate : MonoBehaviour
{
    public float timeToEmpty = 10f;

    public GameObject foodModel;
    public GameObject emptyModel;

    bool isOnTable = false;

    public void OnPlacedOnTable()
    {
        isOnTable = true;
        Invoke(nameof(MakeEmpty), timeToEmpty);
    }

    public void OnPickedUp()
    {
        Debug.Log("Colocado na mesa!");
        isOnTable = false;
        CancelInvoke(nameof(MakeEmpty));
    }

    void MakeEmpty()
    {
        Debug.Log("FICOU VAZIO!");
        if (!isOnTable) return;

        if (foodModel != null) foodModel.SetActive(false);
        if (emptyModel != null) emptyModel.SetActive(true);
    }
}