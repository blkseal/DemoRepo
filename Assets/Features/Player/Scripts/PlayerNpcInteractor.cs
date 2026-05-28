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
        if (playerCamera == null)
        {
            InteractionPromptUI.Instance.Hide();
            return;
        }

        if (PlayerInteractionState.IsLocked)
        {
            InteractionPromptUI.Instance.Hide();
            return;
        }

        var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (!Physics.Raycast(ray, out var hit, interactDistance, interactableMask, QueryTriggerInteraction.Ignore))
        {
            InteractionPromptUI.Instance.Hide();
            return;
        }

        var phoneCall = hit.collider.GetComponentInParent<PhoneCallInteractable>();
        if (phoneCall != null)
        {
            if (phoneCall.CanInteract)
            {
                InteractionPromptUI.Instance.Show(phoneCall.PromptText);
            }
            else
            {
                InteractionPromptUI.Instance.Hide();
            }

            if (Input.GetKeyDown(KeyCode.E) && phoneCall.CanInteract)
            {
                phoneCall.Interact();
            }

            return;
        }

        // Check for NPC group or single NPC and show a generic prompt
        var group = hit.collider.GetComponentInParent<NpcGroupInteractable>();
        var npc = hit.collider.GetComponentInParent<NpcInteractable>();

        if (group != null)
        {
            if (group.CanInteract)
            {
                InteractionPromptUI.Instance.Show("Press E to speak");
            }
            else
            {
                InteractionPromptUI.Instance.Hide();
            }
        }
        else if (npc != null)
        {
            if (npc.CanInteract)
            {
                InteractionPromptUI.Instance.Show("Press E to speak");
            }
            else
            {
                InteractionPromptUI.Instance.Hide();
            }
        }
        else
        {
            InteractionPromptUI.Instance.Hide();
        }

        // Only handle input after showing prompt state
        if (!Input.GetKeyDown(KeyCode.E))
        {
            return;
        }

        if (group != null)
        {
            group.Interact();
            return;
        }

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