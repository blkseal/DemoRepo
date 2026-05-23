using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class GarcomMove : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public List<Transform> targetsMesas; // Coloca aqui as tuas mesas/targets
    public Transform cozinhaTarget;      // O ponto da cozinha onde ele dá spawn e despawn

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Inicia o ciclo de vida do garçom assim que ele nasce
        StartCoroutine(CicloDoGarcom());
    }

    void Update()
    {
        // Só tenta ler os dados se o animator e o agente existirem,
        // E CRUCIAL: se o agente estiver ativo e no NavMesh!
        if (animator != null && agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            // Agora é 100% seguro chamar o remainingDistance
            bool estaCaminhando = agent.remainingDistance > agent.stoppingDistance;

            // Passa o valor para as tuas animações
            animator.SetBool("Walk", estaCaminhando);
        }
    }

    IEnumerator CicloDoGarcom()
    {
        while (true) // Ciclo infinito para o comportamento se repetir
        {
            // 1. SPAWN: Garçom aparece na cozinha
            transform.position = cozinhaTarget.position;
            agent.Warp(cozinhaTarget.position); // Garante que o NavMesh sabe onde ele apareceu

            // Ativa o objeto visualmente (caso o tenhas desativado antes)
            AtivarGarcom(true);

            // 2. IR ATÉ ÀS MESAS (Ordem Aleatória)
            // Criamos uma cópia para podermos baralhar os pontos
            List<Transform> pontosParaVisitar = new List<Transform>(targetsMesas);
            BaralharLista(pontosParaVisitar);

            foreach (Transform mesa in pontosParaVisitar)
            {
                // Ordena o agente a ir para a mesa
                agent.SetDestination(mesa.position);

                // Espera até ele chegar à mesa
                yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

                // CHEGOU À MESA: Fica em Idle por 10 segundos
                // (O animator mudará para Idle automaticamente se a velocidade for 0)
                yield return new WaitForSeconds(10f);
            }

            // 3. VOLTAR PARA A COZINHA
            agent.SetDestination(cozinhaTarget.position);

            // Espera chegar à cozinha
            yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance);

            // 4. DESPAWN
            AtivarGarcom(false);

            // 5. ESPERAR 20 SEGUNDOS ANTES DO PRÓXIMO SPAWN
            yield return new WaitForSeconds(20f);
        }
    }

    // Função auxiliar para ativar/desativar o garçom visualmente e fisicamente
    void AtivarGarcom(bool ativar)
    {
        // Desativa o renderizador e as colisões para parecer que deu despawn
        foreach (var renderer in GetComponentsInChildren<Renderer>()) renderer.enabled = ativar;
        if (agent != null) agent.enabled = ativar;
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