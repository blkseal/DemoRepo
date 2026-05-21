using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TableTargetPoint : MonoBehaviour
{
    [SerializeField] private TableSitPoint[] sitPoints;
    [SerializeField] private Vector3 seatOffset = Vector3.zero;

    public bool IsEmpty()
    {
        if (sitPoints == null || sitPoints.Length == 0)
        {
            return false;
        }

        foreach (var sitPoint in sitPoints)
        {
            if (sitPoint != null && sitPoint.IsOccupied)
            {
                return false;
            }
        }

        return true;
    }

    public bool HasFreeSeats(int requiredSeats)
    {
        if (sitPoints == null || requiredSeats <= 0)
        {
            return false;
        }

        var freeSeats = 0;
        foreach (var sitPoint in sitPoints)
        {
            if (sitPoint != null && !sitPoint.IsOccupied)
            {
                freeSeats++;
            }
        }

        return freeSeats >= requiredSeats;
    }

    public TableSitPoint[] ReserveSeats(int requiredSeats)
    {
        if (!HasFreeSeats(requiredSeats))
        {
            return null;
        }

        var reservedSeats = new List<TableSitPoint>(requiredSeats);
        foreach (var sitPoint in sitPoints)
        {
            if (sitPoint == null || sitPoint.IsOccupied)
            {
                continue;
            }

            if (sitPoint.TryOccupy())
            {
                reservedSeats.Add(sitPoint);
            }

            if (reservedSeats.Count >= requiredSeats)
            {
                break;
            }
        }

        return reservedSeats.Count == requiredSeats ? reservedSeats.ToArray() : null;
    }

    public bool TrySeatNpc(NpcInteractable npc)
    {
        if (npc == null || sitPoints == null)
        {
            return false;
        }

        foreach (var sitPoint in sitPoints)
        {
            if (sitPoint == null || !sitPoint.TryOccupy())
            {
                continue;
            }

            npc.SeatAt(sitPoint.transform, seatOffset, this, sitPoint);
            return true;
        }

        return false;
    }

    public void ReleaseSeat(TableSitPoint sitPoint)
    {
        if (sitPoint == null)
        {
            return;
        }

        sitPoint.Release();
    }
}