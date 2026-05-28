using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GerenteController : MonoBehaviour
{
    [Header("Configuracoes de Tempo")]
    [Tooltip("Tempo em minutos para o gerente aparecer.")]
    public float tempoParaAparecerEmMinutos = 10f;
    
    [Header("Pontos de Referencia")]
    public Transform cozinhaSpawnPoint;
    public Transform portaSaidaTarget;
    public Transform pontoX;
    
    [Header("Referencias de NPC")]
    public GarcomMove garcom;
    
    private NavMeshAgent agent;
    private Animator animator;
    private bool eventoIniciado = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // Garante que o gerente comeca desativado se for um objeto na cena
        // Ou simplemente espera o tempo se ja estiver ativo
        StartCoroutine(TimerSequencia());
    }

    IEnumerator TimerSequencia()
    {
        Debug.Log($"[Gerente] Aguardando {tempoParaAparecerEmMinutos} minutos para iniciar o evento...");
        yield return new WaitForSeconds(tempoParaAparecerEmMinutos * 60f);
        
        // Antes de comecar o evento do Gerente, fazemos todos os clientes sairem
        FazerClientesSairem();
        
        StartCoroutine(ExecutarEvento());
    }

    void FazerClientesSairem()
    {
        Debug.Log("[Gerente] Fazendo todos os clientes darem despawn imediato...");
        
        // Parar o spawn de novos NPCs
        NpcSpawnManager spawnManager = Object.FindFirstObjectByType<NpcSpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.StopAllCoroutines(); 
            Debug.Log("[Gerente] Spawn de NPCs parado.");
        }

        // Fazer os que ja estao na cena darem despawn
        NpcInteractable[] todosNpcs = Object.FindObjectsByType<NpcInteractable>(FindObjectsSortMode.None);
        foreach (var npc in todosNpcs)
        {
            if (npc != null)
            {
                // MUITO IMPORTANTE: Nao remover o garcom!
                if (garcom != null && npc.gameObject == garcom.gameObject)
                {
                    continue;
                }

                // Despawn imediato para os outros
                npc.Despawn();
            }
        }
    }

    IEnumerator ExecutarEvento()
    {
        eventoIniciado = true;
        Debug.Log("[Gerente] O evento comecou! O gerente esta vindo...");

        // 1. O garcom vai para o Ponto X
        if (garcom != null && pontoX != null)
        {
            Debug.Log("[Gerente] Enviando garcom para o Ponto X...");
            garcom.IrPara(pontoX);
        }

        // 2. Aparecer na cozinha
        if (cozinhaSpawnPoint != null)
        {
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.Warp(cozinhaSpawnPoint.position);
            }
            else
            {
                transform.position = cozinhaSpawnPoint.position;
            }
        }

        // Ativar visual se estiver escondido
        AtivarVisual(true);

        // 3. Ir ate o garcom (que estara a caminho do ou no Ponto X)
        if (garcom != null)
        {
            while (!garcom.EstaMorto)
            {
                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(garcom.transform.position);
                }

                // Se chegar perto o suficiente (perseguicao automatica)
                float distGarcom = Vector3.Distance(transform.position, garcom.transform.position);

                if (distGarcom <= 2.5f)
                {
                    break;
                }

                yield return new WaitForSeconds(0.5f);
            }

            // 4. Matar o garcom
            if (!garcom.EstaMorto)
            {
                agent.isStopped = true;
                
                // Rotacionar para o garcom
                Vector3 direction = (garcom.transform.position - transform.position).normalized;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction);

                // Animacao de atirar
                animator.SetTrigger("Shoot");
                yield return new WaitForSeconds(0.5f); // Pequeno delay para o "tiro"
                
                garcom.Morrer();
                yield return new WaitForSeconds(2f); // Espera um pouco apos matar
            }
        }

        // 4. Sair pela porta dancando
        if (portaSaidaTarget != null)
        {
            Debug.Log("[Gerente] Saindo dancando!");
            animator.SetBool("IsDancing", true);
            
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.speed = 1.5f; // Dancando costuma ser mais devagar
                agent.SetDestination(portaSaidaTarget.position);
            }

            yield return new WaitUntil(() => agent == null || !agent.isActiveAndEnabled || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));
            
            Debug.Log("[Gerente] Evento finalizado. Sumindo...");
            AtivarVisual(false);
            Destroy(gameObject, 1f);
        }
    }

    void AtivarVisual(bool ativar)
    {
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        foreach (var r in rs) r.enabled = ativar;
        
        if (agent != null && agent.isOnNavMesh) agent.isStopped = !ativar;
    }

    void Update()
    {
        if (!eventoIniciado || agent == null || animator == null) return;

        // Atualizar velocidade para o animator (se nao estiver dancando)
        if (!animator.GetBool("IsDancing"))
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }
    }
}
