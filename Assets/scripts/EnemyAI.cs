using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // --- Referências Essenciais ---
    private NavMeshAgent agent;
    private Animator animator;
    private Transform player; // Referência ao jogador (Target)

    // --- Configurações de Comportamento (Ajustar no Inspector) ---
    [Header("Distâncias")]
    public float patrolRadius = 20f; // Raio máximo de patrulha
    public float chaseRange = 10f;  // Distância para começar a perseguir
    public float attackRange = 2f;   // Distância para iniciar o ataque
    public float timeBetweenAttacks = 2f; // Tempo entre ataques

    [Header("Velocidades")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3.5f;

    // --- Estados da IA ---
    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool alreadyAttacked;
    private bool playerInAttackRange;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Encontrar o jogador (Certifique-se que o jogador tem a Tag "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Objeto com Tag 'Player' não encontrado! IA não funciona.");
        }

        agent.speed = patrolSpeed;
    }

    private void Update()
    {
        if (player == null || agent == null) return;

        // Calcula a distância para o jogador
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Verifica os estados do jogador
        playerInAttackRange = distanceToPlayer <= attackRange;

        if (playerInAttackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Patroling();
        }

        // NOVO: Atualiza os parâmetros do Blend Tree e o estado Idle/Moving
        UpdateAnimator();
    }

    // ------------------------------------------------------------------
    // Comportamento da IA
    // ------------------------------------------------------------------

    private void Patroling()
    {
        agent.speed = patrolSpeed;

        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        // Verifica se o inimigo chegou ao ponto de patrulha
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Calcula um ponto aleatório dentro do raio de patrulha
        float randomZ = Random.Range(-patrolRadius, patrolRadius);
        float randomX = Random.Range(-patrolRadius, patrolRadius);

        // Calcula a posição do novo ponto de patrulha
        Vector3 randomDirection = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        NavMeshHit hit;
        // Verifica se o ponto é válido (está no NavMesh)
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.speed = chaseSpeed;

        // Move o NavMeshAgent na direção do jogador
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Para o movimento para atacar
        agent.SetDestination(transform.position);

        // Garante que o inimigo está virado para o jogador
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        if (!alreadyAttacked)
        {
            // Dispara a animação de ataque (Attack Trigger)
            animator.SetTrigger("Attack");

            // AQUI: Lógica de dano ao jogador seria aplicada (ex: Invoke("ApplyDamage", 0.5f))

            alreadyAttacked = true;
            // Define um cooldown para o próximo ataque
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // ------------------------------------------------------------------
    // Controlo de Animações (Blend Tree)
    // ------------------------------------------------------------------

    private void UpdateAnimator()
    {
        // NOVO: Calcula a velocidade na coordenada local (necessário para o Blend Tree 2D)
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);

        // NOVO: Valores de velocidade para alimentar os parâmetros MoveX e MoveZ (Float)
        float x = localVelocity.x;
        float z = localVelocity.z;

        // Alimenta o Blend Tree (MoveX e MoveZ) com suavização
        animator.SetFloat("MoveX", x, 0.1f, Time.deltaTime);
        animator.SetFloat("MoveZ", z, 0.1f, Time.deltaTime);

        // NOVO: Controla o parâmetro isMoving (Bool) para a transição Idle <-> Blend Tree
        bool currentlyMoving = agent.velocity.magnitude > 0.1f;
        animator.SetBool("isMoving", currentlyMoving);
    }
}