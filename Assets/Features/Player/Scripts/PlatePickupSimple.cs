using UnityEngine;

public class PlatePickupSimple : MonoBehaviour
{
    public Camera cam;
    public Transform handPoint;
    public float distance = 3f;

    Plate currentPlate;
    bool holding = false;

    int tableLayerMask;

    void Start()
    {
        tableLayerMask = LayerMask.GetMask("Table");
    }

    void Update()
    {
        LookForPlate();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!holding)
                PickPlate();
            else
                PlacePlate();
        }
    }

    void LookForPlate()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distance))
        {
            currentPlate = hit.collider.GetComponent<Plate>();
        }
        else
        {
            currentPlate = null;
        }
    }

    void PickPlate()
    {
        if (currentPlate == null) return;

        currentPlate.transform.SetParent(handPoint);
        currentPlate.transform.localPosition = Vector3.zero;
        currentPlate.transform.localRotation = Quaternion.identity;

        holding = true;
    }

void PlacePlate()
{
    if (heldPlate == null)
    {
        Debug.Log("Sem prato na mão!");
        return;
    }

    if (currentSlot == null)
    {
        Debug.Log("Não estás a olhar para um slot!");
        return;
    }

    if (currentSlot.isOccupied)
    {
        Debug.Log("Slot já ocupado!");
        return;
    }

    heldPlate.transform.position = currentSlot.transform.position;
    heldPlate.transform.rotation = currentSlot.transform.rotation;
    heldPlate.transform.parent = null;

    currentSlot.isOccupied = true;
    heldPlate = null;
}
}