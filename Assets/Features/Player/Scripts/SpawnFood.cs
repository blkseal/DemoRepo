using UnityEngine;

public class PlateCounter : MonoBehaviour
{
    public GameObject platePrefab;
    public Transform spawnPoint;

    bool playerInside = false;
    PlatePlayerSystem player;

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            Instantiate(platePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<PlatePlayerSystem>();

        if (player != null)
        {
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlatePlayerSystem>() != null)
        {
            playerInside = false;
        }
    }
}