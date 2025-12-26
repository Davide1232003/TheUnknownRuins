using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Policia_Controller : MonoBehaviour
{
    // --- Configurações Públicas ---
    [Header("Patrulha e Velocidade")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3.5f;

    [Header("Detecção e Combate")]
    public float chaseRange = 15f;
    public float attackRange = 3f;

    [Header("Patrulha - Wait Time")]
    public float waitTimeAtPoint = 5f;

    [Header("Áudio")]
    public AudioClip shootSound; 
    public AudioClip footstepSound; 

    // --- Referências Privadas ---
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    private Transform playerTarget;
    private Saude_Npc myHealth; 
    private int currentPatrolIndex = 0;

    private float waitCounter = 0f;
    private bool waiting = false;
    private bool isDead = false;

    // --- Máquina de Estados ---
    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        myHealth = GetComponent<Saude_Npc>();

        // Validações iniciais
        if (animator == null) Debug.LogError("Animator em falta!");
        if (myHealth != null) myHealth.OnDie.AddListener(DieLogic);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTarget = playerObj.transform;

        // Inicia Patrulha
        if (agent.isActiveAndEnabled && patrolPoints.Length > 0)
        {
            agent.Warp(transform.position);
            agent.destination = patrolPoints[currentPatrolIndex].position;
            agent.speed = patrolSpeed;
        }
    }

    void Update()
    {
        if (isDead) return; 
        if (animator == null) return; 

        // Animação de Movimento
        bool isMoving = agent.velocity.magnitude > 0.1f && !waiting; // Adicionei !waiting para garantir
        animator.SetBool("IsChasing", isMoving); 

        switch (currentState)
        {
            case State.Patrol: PatrolLogic(); break;
            case State.Chase: ChaseLogic(); break;
            case State.Attack: AttackLogic(); break;
        }
    }

    // --- Lógica de Patrulha (CORRIGIDA) ---
    void PatrolLogic()
    {
        // 1. PRIORIDADE MÁXIMA: Deteção do Jogador
        // Movemos isto para o TOPO. Assim, mesmo que esteja à espera, ele deteta.
        if (playerTarget != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
            if (distanceToPlayer <= chaseRange)
            {
                SwitchState(State.Chase);
                return; // Sai da função imediatamente para começar a perseguição
            }
        }

        // Se não detetou o jogador, continua a lógica normal de patrulha...
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        agent.speed = patrolSpeed;

        // 2. Lógica de Espera (Wait)
        if (waiting)
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0f)
            {
                waiting = false;
                agent.isStopped = false;
                
                if (patrolPoints.Length > 0)
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                }
            }
            return; // Retorna aqui se estiver à espera (mas já verificou o jogador lá em cima)
        }

        // 3. Chegada ao Waypoint
        if (!agent.pathPending && agent.remainingDistance < 0.4f)
        {
            waiting = true;
            waitCounter = waitTimeAtPoint;
            agent.isStopped = true; 
        }
    }

    // --- Lógica de Perseguição ---
    void ChaseLogic()
    {
        // Garante que o agente está ativo
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.destination = playerTarget.position;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        // Se estiver muito perto, Ataca
        if (distanceToPlayer <= attackRange)
        {
            SwitchState(State.Attack);
        }
        // Se o jogador fugir muito, volta a patrulhar
        else if (distanceToPlayer > chaseRange * 1.5f) // Aumentei um pouco a margem para não ficar a trocar rápido demais
        {
            SwitchState(State.Patrol); 
        }
    }

    // --- Lógica de Ataque ---
    void AttackLogic()
    {
        agent.isStopped = true; // Garante que para ao atacar

        if (playerTarget != null)
        {
            // Roda suavemente para o jogador
            Vector3 direction = (playerTarget.position - transform.position).normalized;
            direction.y = 0; // Mantém a rotação apenas no eixo Y
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
        
        // Só toca a animação se o estado atual do Animator não for já o de ataque (opcional, para evitar spam)
        animator.SetTrigger("Attack"); 

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        if (distanceToPlayer > attackRange)
        {
            SwitchState(State.Chase);
        }
    }

    void SwitchState(State newState)
    {
        // Limpeza ao sair do estado anterior
        if (currentState == State.Patrol)
        {
            waiting = false; // Cancela a espera se sair da patrulha
        }

        // Configuração ao entrar no novo estado
        if (newState == State.Chase)
        {
            agent.isStopped = false;
            waiting = false; // Garante que não está à espera
        }
        else if (newState == State.Patrol)
        {
            agent.isStopped = false;
            agent.speed = patrolSpeed;
            // Retoma o caminho para o waypoint atual
            if (patrolPoints.Length > 0)
                agent.destination = patrolPoints[currentPatrolIndex].position;
        }

        currentState = newState;
    }

    // --- Sons (Eventos de Animação) ---
    public void PlayShootSound()
    {
        if (audioSource != null && shootSound != null) audioSource.PlayOneShot(shootSound);
    }
    
    public void PlayFootstepSound()
    {
        if (audioSource != null && footstepSound != null) audioSource.PlayOneShot(footstepSound);
    }

    // --- Morte ---
    private void DieLogic()
    {
        isDead = true; 
        
        if (agent != null)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }
        
        if (animator != null)
        {
            // animator.SetTrigger("Die"); // Caso queira controlar a morte aqui
            // Mas como o Saude_Npc já trata disso, aqui apenas desativamos o script
        }
        
        this.enabled = false;
        Destroy(gameObject, 3f); 
    }
}