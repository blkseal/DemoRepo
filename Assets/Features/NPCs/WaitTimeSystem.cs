using UnityEngine;

public class WaitTimeSystem : MonoBehaviour
{
    [Header("Wait Time")]
    [SerializeField] private float maxWaitTime = 90f;

    [Header("Penalties")]
    [SerializeField] private int suspicionPenalty = 5;
    [SerializeField] private int reviewPenalty = -5;

    [Header("Leave Settings")]
    [SerializeField] private float destroyDelay = 4f;

    private float waitTimer;
    private bool hasComplained;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (hasComplained)
        {
            return;
        }

        waitTimer += Time.deltaTime;

        if (waitTimer >= maxWaitTime)
        {
            CustomerComplains();
        }
    }

    private void CustomerComplains()
    {
        // If this NPC is part of an NPC group (handled by NpcGroupInteractable),
        // skip the individual wait/complain logic here so groups are handled by their own system.
        var group = GetComponentInParent<NpcGroupInteractable>();
        if (group != null)
        {
            Debug.Log($"{name} is part of a group — skipping individual wait complaint.");
            return;
        }

        hasComplained = true;

        Debug.Log($"{name} got tired of waiting and complained!");

        // Aplica as penalidades nas variáveis do Hugo
        if (GameStatusSystem.Instance != null)
        {
            GameStatusSystem.Instance.ApplyInteractionResult(
                suspicionPenalty,
                reviewPenalty
            );
        }

        NpcInteractable npc = GetComponent<NpcInteractable>();

        if (npc != null)
        {
            // CHAMA APENAS ISTO: Ele vai tratar de quem está em pé E de quem está sentado!
            npc.ForceLeaveRestaurant();
        }
    }

    private void MoveAway()
    {
        UnityEngine.AI.NavMeshAgent agent =
            GetComponent<UnityEngine.AI.NavMeshAgent>();

        Animator animator =
            GetComponent<Animator>();

        // Desliga animação sentada
        if (animator != null)
        {
            animator.SetBool("IsSitting", false);

            // reduz velocidade da animação
            animator.speed = 0.8f;
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.enabled = true;

            // movimento mais natural
            agent.speed = 1.2f;
            agent.angularSpeed = 120f;
            agent.acceleration = 4f;

            agent.isStopped = false;

            Vector3 targetPosition =
                transform.position +
                transform.forward * 12f;

            agent.SetDestination(targetPosition);
        }

        Destroy(gameObject, 10f);
    }
}