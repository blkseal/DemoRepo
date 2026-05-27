using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class GarcomMove : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public List<Transform> targetsMesas; // Coloca aqui as tuas mesas/targets
    public Transform cozinhaTarget;      // O ponto da cozinha onde ele dá spawn e despawn

    [Header("Configuração do Target Especial (Tempo)")]
    public Transform targetEspecial;     // O novo ponto para onde ele vai após o tempo acabar
    [Tooltip("Tempo em minutos antes de ir para o target especial.")]
    public float tempoParaMudarEmMinutos = 3f; // Padrão: 3 minutos para testes. Mude para 14 no Inspector depois.

    [Header("Configurações de Rotação (Injetado do NPC)")]
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float stoppedSpeedThreshold = 0.01f;

    private NavMeshAgent agent;
    private Animator animator;
    private NpcInteractable npcInteractable;
    private Renderer[] meusRenderers;

    private float tempoDecorrido = 0f;
    private bool tempoEsgotado = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        npcInteractable = GetComponent<NpcInteractable>();

        meusRenderers = GetComponentsInChildren<Renderer>();

        if (agent != null)
        {
            agent.updateRotation = false;
        }

        StartCoroutine(CicloDoGarcom());
    }

    void Update()
    {
        // Cronômetro global: Só conta se o tempo ainda não esgotou e se o target especial existe
        if (!tempoEsgotado && targetEspecial != null)
        {
            tempoDecorrido += Time.deltaTime;
            if (tempoDecorrido >= (tempoParaMudarEmMinutos * 60f))
            {
                tempoEsgotado = true;
                Debug.LogWarning($"[GarcomMove] Tempo limite de {tempoParaMudarEmMinutos} minutos atingido! Indo para o Target Especial.");
            }
        }

        if (agent == null || animator == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
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

        var targetDirection = speed > stoppedSpeedThreshold ? agent.velocity : Vector3.zero;
        targetDirection.y = 0f;

        if (targetDirection.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        var targetRotation = Quaternion.LookRotation(targetDirection.normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    IEnumerator CicloDoGarcom()
    {
        yield return null;

        // LOOP 1: Ciclo normal de trabalho (Mesas <-> Cozinha)
        // Ele continuará aqui ATÉ o tempo esgotar
        while (!tempoEsgotado)
        {
            if (cozinhaTarget == null)
            {
                Debug.LogError("Cozinha Target não foi atribuído no Inspetor!", this);
                yield return new WaitForSeconds(5f);
                continue;
            }

            // 1. SPAWN: Garçom aparece na cozinha
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.Warp(cozinhaTarget.position);
            }
            else
            {
                transform.position = cozinhaTarget.position;
            }

            AtivarGarcom(true);

            // 2. IR ATÉ ÀS MESAS (Ordem Aleatória)
            List<Transform> pontosParaVisitar = new List<Transform>(targetsMesas);
            BaralharLista(pontosParaVisitar);

            foreach (Transform mesa in pontosParaVisitar)
            {
                if (mesa == null) continue;
                if (tempoEsgotado) break; // Interrompe se o tempo acabar a meio do trajeto entre mesas

                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(mesa.position);
                }

                // Espera chegar à mesa (ou sai se o tempo esgotar)
                yield return new WaitUntil(() => tempoEsgotado || agent == null || !agent.isActiveAndEnabled || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

                if (tempoEsgotado) break;

                // CHEGOU À MESA: Fica em Idle por 10 segundos
                yield return new WaitForSeconds(10f);
            }

            // Se o tempo acabou enquanto ele atendia as mesas, não volta para a cozinha, sai logo do loop
            if (tempoEsgotado) break;

            // 3. VOLTAR PARA A COZINHA
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.SetDestination(cozinhaTarget.position);
            }

            yield return new WaitUntil(() => tempoEsgotado || agent == null || !agent.isActiveAndEnabled || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

            if (tempoEsgotado) break;

            // 4. DESPAWN (Esconde o boneco)
            AtivarGarcom(false);

            // 5. ESPERAR 20 SEGUNDOS ANTES DO PRÓXIMO SPAWN
            // Esta espera também pode ser interrompida se o tempo acabar
            float tempoEsperaCozinha = 0f;
            while (tempoEsperaCozinha < 20f && !tempoEsgotado)
            {
                tempoEsperaCozinha += Time.deltaTime;
                yield return null;
            }
        }

        // ==========================================
        // COMPORTAMENTO APÓS O TEMPO ESGOTAR
        // ==========================================

        // Garante que o garçom está visível e ativo para caminhar até ao destino final
        AtivarGarcom(true);

        if (targetEspecial != null)
        {
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.SetDestination(targetEspecial.position);
            }

            // Espera até ele chegar ao destino final de 14min
            yield return new WaitUntil(() => agent == null || !agent.isActiveAndEnabled || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

            // O que ele faz ao chegar lá? (Exemplo: Fica parado em Idle)
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }

            // Podes adicionar animações ou interações aqui se quiseres!
            Debug.Log("[GarcomMove] Cheguei ao destino especial de tempo esgotado.");
        }
        else
        {
            Debug.LogError("Tempo esgotou mas o 'Target Especial' não foi atribuído no Inspector!");
        }
    }

    void AtivarGarcom(bool ativar)
    {
        if (meusRenderers == null || meusRenderers.Length == 0)
        {
            meusRenderers = GetComponentsInChildren<Renderer>();
        }

        foreach (var renderer in meusRenderers)
        {
            if (renderer != null)
            {
                renderer.enabled = ativar;
            }
        }

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = !ativar;
        }
    }

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