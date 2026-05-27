using UnityEngine;

public class PlayerPlateSystem : MonoBehaviour
{
    public Transform handPoint;

    GameObject heldPlate;

    PlatePickupSource nearbySource;
    TableZone nearbyTable;
    SinkZone nearbySink;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 🍽️ PEGAR DO BALCÃO (SPAWN)
            if (heldPlate == null && nearbySource != null)
            {
                SpawnPlate();
                return;
            }

            // 🚰 LIXO
            if (heldPlate != null && nearbySink != null)
            {
                Destroy(heldPlate);
                heldPlate = null;
                return;
            }

            // 🪑 MESA
            if (nearbyTable != null)
            {
                if (heldPlate == null)
                    TakeFromTable();
                else
                    PlaceOnTable();
            }
        }
    }

    void SpawnPlate()
    {
        heldPlate = Instantiate(
            nearbySource.platePrefab,
            handPoint.position,
            handPoint.rotation
        );

        heldPlate.transform.SetParent(handPoint);
        heldPlate.transform.localPosition = Vector3.zero;
        heldPlate.transform.localRotation = Quaternion.identity;
        heldPlate.transform.localScale = Vector3.one;

        Plate plate = heldPlate.GetComponent<Plate>();
        if (plate != null)
            plate.OnPickedUp();
    }

    void PlaceOnTable()
    {
        // Cache all seats in the scene for this placement attempt
        // Using FindObjectsByType as recommended by modern Unity versions
        TableSitPoint[] allSeatsInScene = Object.FindObjectsByType<TableSitPoint>(FindObjectsSortMode.None);

        // Pass 1: Prioritize slots whose CLOSEST chair is occupied by a customer
        foreach (TableSlot slot in nearbyTable.slots)
        {
            if (!slot.occupied && IsCustomerAtSlot(slot, allSeatsInScene))
            {
                PlaceInSlot(slot);
                return;
            }
        }

        // Pass 2: Fallback to any empty slot
        foreach (TableSlot slot in nearbyTable.slots)
        {
            if (!slot.occupied)
            {
                PlaceInSlot(slot);
                return;
            }
        }

        Debug.Log("Mesa cheia!");
    }

    bool IsCustomerAtSlot(TableSlot slot, TableSitPoint[] allSeats)
    {
        if (allSeats == null || allSeats.Length == 0) return false;

        // Use the snap point if available for better distance accuracy
        Vector3 slotPos = slot.snapPoint != null ? slot.snapPoint.position : slot.transform.position;

        TableSitPoint closestSeat = null;
        float minSeatDist = float.MaxValue;

        // Find the chair that is physically closest to this plate slot
        foreach (var seat in allSeats)
        {
            float d = Vector3.Distance(slotPos, seat.transform.position);
            if (d < minSeatDist)
            {
                minSeatDist = d;
                closestSeat = seat;
            }
        }

        // Only prioritize if the closest chair is actually occupied
        // Radius of 2.5m is used to ensure we catch chairs even at large tables
        return closestSeat != null && closestSeat.IsOccupied && minSeatDist < 2.5f;
    }

    void PlaceInSlot(TableSlot slot)
    {
        heldPlate.transform.SetParent(null);

        heldPlate.transform.position = slot.snapPoint.position;
        heldPlate.transform.rotation = slot.snapPoint.rotation;
        heldPlate.transform.localScale = Vector3.one;

        slot.occupied = true;
        slot.currentPlate = heldPlate;

        Plate plate = heldPlate.GetComponent<Plate>();
        if (plate != null)
        {
            plate.OnPlacedOnTable();
            nearbyTable.NotifyPlatePlaced(plate);
        }

        heldPlate = null;
    }

    void TakeFromTable()
    {
        foreach (TableSlot slot in nearbyTable.slots)
        {
            if (slot.occupied && slot.currentPlate != null)
            {
                heldPlate = slot.currentPlate;

                heldPlate.transform.SetParent(handPoint);
                heldPlate.transform.localPosition = Vector3.zero;
                heldPlate.transform.localRotation = Quaternion.identity;
                heldPlate.transform.localScale = Vector3.one;

                slot.occupied = false;
                slot.currentPlate = null;

                Plate plate = heldPlate.GetComponent<Plate>();
                if (plate != null)
                    plate.OnPickedUp();

                return;
            }
        }

        Debug.Log("Mesa vazia!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlatePickupSource>())
            nearbySource = other.GetComponent<PlatePickupSource>();

        if (other.GetComponent<TableZone>())
            nearbyTable = other.GetComponent<TableZone>();

        if (other.GetComponent<SinkZone>())
            nearbySink = other.GetComponent<SinkZone>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlatePickupSource>())
            nearbySource = null;

        if (other.GetComponent<TableZone>())
            nearbyTable = null;

        if (other.GetComponent<SinkZone>())
            nearbySink = null;
    }
}