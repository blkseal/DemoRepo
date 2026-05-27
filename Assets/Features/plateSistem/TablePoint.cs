using System;
using UnityEngine;

public class TableZone : MonoBehaviour
{
    public TableSlot[] slots;
    public event Action<Plate> OnPlatePlaced;

    public void NotifyPlatePlaced(Plate plate)
    {
        OnPlatePlaced?.Invoke(plate);
    }
}