using UnityEngine;
using UnityEngine.AI;

public class NPCAnimationController : MonoBehaviour
{
    public NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float stoppedSpeedThreshold = 0.01f;
    private Animator animator;
    private NpcInteractable npcInteractable;

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        npcInteractable = GetComponent<NpcInteractable>();

        if (player == null)
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (agent != null)
        {
            agent.updateRotation = false;
        }

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (agent == null || animator == null)
        {
            return;
        }

        if (npcInteractable != null && npcInteractable.IsSitting)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        var targetDirection = speed > stoppedSpeedThreshold ? agent.velocity : player != null ? player.position - transform.position : Vector3.zero;
        targetDirection.y = 0f;

        if (targetDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        var targetRotation = Quaternion.LookRotation(targetDirection.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}