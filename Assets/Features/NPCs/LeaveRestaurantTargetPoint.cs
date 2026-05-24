using UnityEngine;

public class LeaveRestaurantTargetPoint : MonoBehaviour
{
    [SerializeField] private string npcTag = "NPC";

    private void OnTriggerEnter(Collider other)
    {
        if (other == null)
        {
            return;
        }

        var rootObject = other.transform.root.gameObject;
        if (!rootObject.CompareTag(npcTag))
        {
            return;
        }

        Destroy(rootObject);
    }
}