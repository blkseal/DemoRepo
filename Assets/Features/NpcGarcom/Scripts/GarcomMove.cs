using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class GarcomMove : MonoBehaviour
{
    [Header("Configuracoes de Movimento")]
    public List<Transform> targetsMesas; 
    public Transform cozinhaTarget;      

    [Header("Configuracao do Target Especial (Tempo)")]
    public Transform targetEspecial;     
    [Tooltip("Tempo em minutos antes de ir para o target especial.")]
    public float tempoParaMudarEmMinutos = 3f; 

    [Header("Configuracoes de Rotacao (Injetado do NPC)")]
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float stoppedSpeedThreshold = 0.01f;

    private NavMeshAgent agent;
    private Animator animator;
    private NpcInteractable npcInteractable;
    private Renderer[] meusRenderers;

    private float tempoDecorrido = 0f;
    private bool tempoEsgotado = false;
    private bool estaMorto = false;

    public bool EstaMorto => estaMorto;

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
        if (estaMorto) return;

        if (!tempoEsgotado && targetEspecial != null)
        {
            tempoDecorrido += Time.deltaTime;
            if (tempoDecorrido >= (tempoParaMudarEmMinutos * 60f))
            {
                tempoEsgotado = true;
                Debug.LogWarning("[GarcomMove] Tempo limite atingido! Indo para o Target Especial.");
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

    public void IrPara(Transform destino)
    {
        if (estaMorto) return;
        
        StopAllCoroutines();
        AtivarGarcom(true);
        
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(destino.position);
        }
    }

    public void Morrer()
    {
        if (estaMorto) return;
        
        estaMorto = true;
        StopAllCoroutines();

        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
            animator.SetFloat("Speed", 0f);
        }

        Debug.Log("[GarcomMove] O garcom morreu!");
    }

    IEnumerator CicloDoGarcom()
    {
        yield return null;

        while (!tempoEsgotado && !estaMorto)
        {
            if (cozinhaTarget == null)
            {
                Debug.LogError("Cozinha Target nao foi atribuido no Inspetor!", this);
                yield return new WaitForSeconds(5f);
                continue;
            }

            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.Warp(cozinhaTarget.position);
            }
            else
            {
                transform.position = cozinhaTarget.position;
            }

            AtivarGarcom(true);

            List<Transform> pontosParaVisitar = new List<Transform>(targetsMesas);
            BaralharLista(pontosParaVisitar);

            foreach (Transform mesa in pontosParaVisitar)
            {
                if (mesa == null) continue;
                if (tempoEsgotado || estaMorto) break;

                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(mesa.position);
                }

                yield return new WaitUntil(() => estaMorto || tempoEsgotado || agent == null || !agent.isActiveAndEnabled || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

                if (tempoEsgotado || estaMorto) break;

                yield return new WaitForSeconds(10f);
            }

            if (tempoEsgotado || estaMorto) break;

            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.SetDestination(cozinhaTarget.position);
            }

            yield return new WaitUntil(() => estaMorto || tempoEsgotado || agent == null || !agent.isActiveAndEnabled || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

            if (tempoEsgotado || estaMorto) break;

            AtivarGarcom(false);

            float tempoEsperaCozinha = 0f;
            while (tempoEsperaCozinha < 20f && !tempoEsgotado && !estaMorto)
            {
                tempoEsperaCozinha += Time.deltaTime;
                yield return null;
            }
        }

        if (estaMorto) yield break;

        AtivarGarcom(true);

        if (targetEspecial != null)
        {
            if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            {
                agent.SetDestination(targetEspecial.position);
            }

            yield return new WaitUntil(() => estaMorto || agent == null || !agent.isActiveAndEnabled || (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance));

            if (estaMorto) yield break;

            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }

            Debug.Log("[GarcomMove] Cheguei ao destino especial.");
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
