using UnityEngine;

public class PlatePlayerSystem : MonoBehaviour
{
    public Transform handPoint;

    GameObject nearbyPlate;
    TableZone nearbyTableZone;
    SinkZone nearbySink;

    GameObject heldPlate;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldPlate == null)
                PickPlate();
            else
                PlacePlate();
        }
    }

    void PickPlate()
    {
        if (nearbyPlate == null) return;

        Plate plate = nearbyPlate.GetComponent<Plate>();
        if (plate != null)
            plate.OnPickedUp();

        heldPlate = nearbyPlate;

        heldPlate.transform.SetParent(handPoint);
        heldPlate.transform.localPosition = Vector3.zero;
        heldPlate.transform.localRotation = Quaternion.identity;
        heldPlate.transform.localScale = Vector3.one;
    }

    void PlacePlate()
{
    if (heldPlate == null) return;

    TableSlot slot = GetFreeSlot();
    if (slot == null)
    {
        Debug.Log("Mesa cheia!");
        return;
    }

    Transform snap = slot.transform.Find("SnapPoint");
    if (snap == null)
    {
        Debug.Log("SnapPoint em falta!");
        return;
    }

    Plate plate = heldPlate.GetComponent<Plate>();
    if (plate != null)
        plate.OnPlacedOnTable();

    heldPlate.transform.SetParent(null);
    heldPlate.transform.position = snap.position;
    heldPlate.transform.rotation = snap.rotation;
    heldPlate.transform.localScale = Vector3.one;

    slot.occupied = true;
    heldPlate = null;
}

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Plate>())
            nearbyPlate = other.gameObject;

        if (other.GetComponent<TableZone>())
            nearbyTableZone = other.GetComponent<TableZone>();

        if (other.GetComponent<SinkZone>())
            nearbySink = other.GetComponent<SinkZone>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Plate>())
            nearbyPlate = null;

        if (other.GetComponent<TableZone>())
            nearbyTableZone = null;

        if (other.GetComponent<SinkZone>())
            nearbySink = null;
    }
    TableSlot GetFreeSlot()
{
    TableSlot[] slots =
        Object.FindObjectsByType<TableSlot>(FindObjectsSortMode.None);

    foreach (TableSlot s in slots)
    {
        if (!s.occupied)
            return s;
    }

    return null;
}
}