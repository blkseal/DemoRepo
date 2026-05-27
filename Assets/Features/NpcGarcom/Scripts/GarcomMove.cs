using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class GarcomMove : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public List<Transform> targetsMesas; // Coloca aqui as tuas mesas/targets
    public Transform cozinhaTarget;      // O ponto da cozinha onde ele dá spawn e despawn

    [Header("Configurações de Rotação (Injetado do NPC)")]
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float stoppedSpeedThreshold = 0.01f;

    private NavMeshAgent agent;
    private Animator animator;
    private NpcInteractable npcInteractable;
    private Renderer[] meusRenderers; // Guardado aqui para não falhar

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        npcInteractable = GetComponent<NpcInteractable>();

        // Busca os renderizadores logo no início para garantir que os guarda na memória
        meusRenderers = GetComponentsInChildren<Renderer>();

        // Desativa a rotação automática do NavMeshAgent para usarmos a nossa rotação suave
        if (agent != null)
        {
            agent.updateRotation = false;
        }

        // Inicia o ciclo de vida do garçom assim que ele nasce
        StartCoroutine(CicloDoGarcom());
    }

    void Update()
    {
        // Só tenta ler os dados se o animator e o agente existirem,
        // E CRUCIAL: se o agente estiver ativo e no NavMesh!
        if (agent == null || animator == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
        {
            return;
        }

        // Se o NPC estiver sentado (comportamento do primeiro script)
        if (npcInteractable != null && npcInteractable.IsSitting)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        // Pega a velocidade real do agente do NavMesh (assim como o NPC fazia)
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);

        // Lógica de rotação suave: foca apenas na velocidade do agente (sem olhar para o player)
        var targetDirection = speed > stoppedSpeedThreshold ? agent.velocity : Vector3.zero;
        targetDirection.y = 0f; // Impede o garçom de se inclinar em rampas

        // Se a direção for válida, aplica a rotação suave
        if (targetDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        var targetRotation = Quaternion.LookRotation(targetDirection.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    IEnumerator CicloDoGarcom()
    {
        // Dá folga de 1 frame para o Unity carregar todos os componentes na cena antes de iniciar
        yield return null;

        while (true) // Ciclo infinito para o comportamento se repetir
        {
            // Se o alvo da cozinha não estiver definido, avisa para evitar erros
            if (cozinhaTarget == null)
            {
                Debug.LogError("Cozinha Target não foi atribuído no Inspetor!", this);
                yield return new WaitForSeconds(5f);
                continue;
            }

            // 1. SPAWN: Garçom aparece na cozinha
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.Warp(cozinhaTarget.position); // Garante que o NavMesh sabe onde ele apareceu
            }
            else
            {
                transform.position = cozinhaTarget.position;
            }

            // Ativa o objeto visualmente
            AtivarGarcom(true);

            // 2. IR ATÉ ÀS MESAS (Ordem Aleatória)
            List<Transform> pontosParaVisitar = new List<Transform>(targetsMesas);
            BaralharLista(pontosParaVisitar);

            foreach (Transform mesa in pontosParaVisitar)
            {
                if (mesa == null) continue;

                // Ordena o agente a ir para a mesa
                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(mesa.position);
                }

                // Espera até ele chegar à mesa
                yield return new WaitUntil(() => agent == null || !agent.isActiveAndEnabled || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

                // CHEGOU À MESA: Fica em Idle por 10 segundos
                yield return new WaitForSeconds(10f);
            }

            // 3. VOLTAR PARA A COZINHA
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.SetDestination(cozinhaTarget.position);
            }

            // Espera chegar à cozinha
            yield return new WaitUntil(() => agent == null || !agent.isActiveAndEnabled || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

            // 4. DESPAWN (Esconde o boneco)
            AtivarGarcom(false);

            // 5. ESPERAR 20 SEGUNDOS ANTES DO PRÓXIMO SPAWN
            yield return new WaitForSeconds(20f);
        }
    }

    // Função revisada: desliga apenas a renderização visual para não quebrar a física do NavMesh
    void AtivarGarcom(bool ativar)
    {
        // Se por acaso a lista estiver vazia, tenta buscar novamente
        if (meusRenderers == null || meusRenderers.Length == 0)
        {
            meusRenderers = GetComponentsInChildren<Renderer>();
        }

        // Liga/Desliga os componentes visuais
        foreach (var renderer in meusRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = ativar;
            }
        }

        // Evitamos desligar o 'agent.enabled' aqui para o NavMesh não perder o rastro do boneco e bugar o Warp
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = !ativar; // Se não estiver ativo, ele apenas para de andar
        }
    }

    // Algoritmo simples para baralhar a lista de mesas e torná-las aleatórias
    void BaralharLista(List<Transform> lista)
    {
        for (int i = 0; i < lista.Count; i++)
        {
            Transform temp = lista[i];
            int randomIndex = Random.Range(i, lista.Count);
            lista[i] = lista[randomIndex];
            lista[randomIndex] = temp;
        }
    }
}