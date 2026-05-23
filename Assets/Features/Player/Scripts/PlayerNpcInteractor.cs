using UnityEngine;

public class PlayerNpcInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableMask = ~0;
    [SerializeField] private Behaviour[] disableWhileLocked;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        PlayerInteractionState.LockChanged += HandleLockChanged;
    }

    private void OnDestroy()
    {
        PlayerInteractionState.LockChanged -= HandleLockChanged;
    }

    private void Start()
    {
        HandleLockChanged(PlayerInteractionState.IsLocked);
    }

    private void Update()
    {
        if (PlayerInteractionState.IsLocked || playerCamera == null)
        {
            return;
        }

        if (!Input.GetKeyDown(KeyCode.E))
        {
            return;
        }

        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (!Physics.Raycast(ray, out var hit, interactDistance, interactableMask, QueryTriggerInteraction.Ignore))
        {
            return;
        }

        var group = hit.collider.GetComponentInParent<NpcGroupInteractable>();
        if (group != null)
        {
            group.Interact();
            return;
        }

        var npc = hit.collider.GetComponentInParent<NpcInteractable>();
        if (npc == null)
        {
            return;
        }

        npc.Interact();
    }

    private void HandleLockChanged(bool locked)
    {
        if (disableWhileLocked != null)
        {
            foreach (var behaviour in disableWhileLocked)
            {
                if (behaviour != null)
                {
                    behaviour.enabled = !locked;
                }
            }
        }

        Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = locked;
    }
}