using UnityEngine;

public class TableSitPoint : MonoBehaviour
{
    [SerializeField] private bool occupied;

    public bool IsOccupied => occupied;

    public bool TryOccupy()
    {
        if (occupied)
        {
            return false;
        }

        occupied = true;
        return true;
    }

    public void Release()
    {
        occupied = false;
    }
}