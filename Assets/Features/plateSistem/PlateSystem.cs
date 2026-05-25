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
        foreach (TableSlot slot in nearbyTable.slots)
        {
            if (!slot.occupied)
            {
                heldPlate.transform.SetParent(null);

                heldPlate.transform.position = slot.snapPoint.position;
                heldPlate.transform.rotation = slot.snapPoint.rotation;
                heldPlate.transform.localScale = Vector3.one;

                slot.occupied = true;
                slot.currentPlate = heldPlate;

                Plate plate = heldPlate.GetComponent<Plate>();
                if (plate != null)
                    plate.OnPlacedOnTable();

                heldPlate = null;
                return;
            }
        }

        Debug.Log("Mesa cheia!");
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