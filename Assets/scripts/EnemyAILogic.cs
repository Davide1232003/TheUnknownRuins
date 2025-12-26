using UnityEngine;
using UnityEngine.AI;
using System.Collections;

// 1. Definição dos Estados do NPC
public enum DefenderState { WAITING, ROTATING, PATROLING, CHASING, ATTACKING }

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAILogic : MonoBehaviour
{
    // Variáveis públicas (Configurações e Ligações no Inspector)
    public Transform[] waypoints;
    public Transform playerTarget; 

    // --- Configuração do Disparo ---
    public GameObject projectilePrefab; // O Prefab do projétil (liga-se no Inspector)
    public Transform firePoint;         // O ponto onde o projétil será instanciado (liga-se no Inspector)
    public float projectileSpeed = 15f; // Velocidade do projétil
    // -------------------------------------

    // --- Configuração do Áudio ---
    public AudioClip shootSoundClip; 
    // ------------------------------------

    // Configurações
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3.5f;
    public float stoppingDistance = 1.0f;
    public float waitDuration = 5.0f;
    public float rotationSpeed = 3.0f;
    public float attackRange = 10.0f; // Sugestão: Aumentado para parar mais longe
    public float timeBetweenAttacks = 1.5f;


    private NavMeshAgent agent;
    private Animator anim;
    private AudioSource audioSource; 
    private int currentWaypointIndex = 0;
    private bool waitingAtWaypoint = false;
    private DefenderState currentState = DefenderState.PATROLING;
    private float timeSinceLastAttack = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Inicialização do AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        agent.stoppingDistance = stoppingDistance;
        agent.speed = patrolSpeed;

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        switch (currentState)
        {
            case DefenderState.WAITING:
                break;

            case DefenderState.ROTATING:
                HandleRotation();
                break;

            case DefenderState.PATROLING:
                PatrolLogic();
                CheckArrivalAndStartWait();
                break;

            case DefenderState.CHASING:
                HandleChase();
                break;

            case DefenderState.ATTACKING:
                HandleAttackState();
                break;
        }
    }


    // ------------ DETECTION / TRIGGERS --------------

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && playerTarget != null)
        {
            // Cancela Patrulha/Espera se estiver ativo
            if (currentState == DefenderState.WAITING || currentState == DefenderState.ROTATING)
            {
                StopAllCoroutines();
                waitingAtWaypoint = false;
            }
            
            Debug.Log("Jogador detectado! Iniciando Perseguição.");

            agent.isStopped = false;
            agent.speed = chaseSpeed;
            anim.SetBool("IsChasing", true);

            currentState = DefenderState.CHASING;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jogador saiu da zona de deteção! Voltando à Patrulha.");

            anim.ResetTrigger("Attack");
            
            currentState = DefenderState.PATROLING;

            agent.isStopped = false;
            agent.speed = patrolSpeed;
            anim.SetBool("IsChasing", true); 

            agent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }


    // ----------- MÉTODO CHAMADO PARA DISPARAR O PROJÉTIL E O SOM -----------

    // ESTE MÉTODO É CHAMADO PELO EVENTO DE ANIMAÇÃO DO ATAQUE
    public void FireWeapon()
    {
        // 1. Tocar o som do disparo (Sincronizado)
        if (audioSource != null && shootSoundClip != null)
        {
            audioSource.PlayOneShot(shootSoundClip);
        }
        
        // Verificações de segurança antes de instanciar a bala
        if (playerTarget == null) { Debug.LogError("FireWeapon Error: playerTarget is NULL."); return; }
        if (projectilePrefab == null) { Debug.LogError("FireWeapon Error: projectilePrefab is NULL."); return; }
        if (firePoint == null) { Debug.LogError("FireWeapon Error: firePoint is NULL."); return; }
        
        // 2. Instanciar o Projétil
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        
        // 3. Calcular a direção (Mira direta no jogador)
        Vector3 targetDirection = (playerTarget.position - firePoint.position).normalized;
        
        // 4. Obter o Rigidbody
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError("FireWeapon Error: Rigidbody is NULL on the INSTANTIATED projectile. Check your Prefab!");
            Destroy(projectile); 
            return;
        }
        
        // CORRIGIDO: Aplica a velocidade usando rb.velocity
        rb.linearVelocity = targetDirection * projectileSpeed; 
        
        Debug.Log("FireWeapon SUCESSO! Projétil lançado e Som reproduzido.");
    }

    // ----------- ATTACK (Parado e a Disparar) -----------

    void HandleAttackState()
    {
        if (playerTarget == null)
        {
            currentState = DefenderState.PATROLING;
            return;
        }

        // 1. Olhar para o alvo (Rotação Suave para manter a mira)
        Quaternion targetRotation = Quaternion.LookRotation(playerTarget.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed * 2.0f);


        // 2. Verificar se o jogador saiu do alcance de ataque
        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance > attackRange + 0.1f)
        {
            Debug.Log("Jogador fora do alcance de ataque. Voltando a perseguir.");
            anim.ResetTrigger("Attack");
            currentState = DefenderState.CHASING;
            agent.isStopped = false; 
            return;
        }

        // 3. Executar ataque com cooldown
        if (timeSinceLastAttack >= timeBetweenAttacks)
        {
            anim.SetTrigger("Attack"); // Dispara a animação que chama FireWeapon()
            timeSinceLastAttack = 0f;
            
        }
    }


    // ------------------ CHASE (Perseguição) -------------------

    void HandleChase()
    {
        if (playerTarget == null)
        {
            currentState = DefenderState.PATROLING;
            return;
        }

        // 1. Olhar para o alvo (Rotação imediata enquanto corre)
        transform.LookAt(playerTarget);


        // 2. Verificar distância para Ataque
        float distance = Vector3.Distance(transform.position, playerTarget.position);

        if (distance <= attackRange)
        {
            // Chegou ao alcance de ataque, parar e atacar
            agent.isStopped = true;
            anim.SetBool("IsChasing", false);
            anim.ResetTrigger("Attack");
            
            currentState = DefenderState.ATTACKING;
            
            // Permite um ataque imediato se o cooldown tiver terminado
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                 anim.SetTrigger("Attack");
                 timeSinceLastAttack = 0f;
            }
            return;
        }

        // 3. Perseguir (se estiver fora do alcance de ataque)
        agent.isStopped = false;
        agent.SetDestination(playerTarget.position);
        agent.speed = chaseSpeed;
        anim.SetBool("IsChasing", true);
    }

 
    // -------- PATROL (Patrulha) --------

    void PatrolLogic()
    {
        agent.speed = patrolSpeed;
        anim.SetBool("IsChasing", true);
    }


    void CheckArrivalAndStartWait()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
        {
            if (agent.velocity.sqrMagnitude < 0.1f && !waitingAtWaypoint)
            {
                StartCoroutine(WaitAtWaypoint());
            }
        }
    }


    // --------- ROTATE (Girar para o próximo Waypoint) ---------

    void HandleRotation()
    {
        Vector3 targetDirection = waypoints[currentWaypointIndex].position - transform.position;
        targetDirection.y = 0; 

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        
        anim.SetBool("IsChasing", false);
        agent.speed = 0f;

        if (Quaternion.Angle(transform.rotation, targetRotation) < 1.0f)
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;

            currentState = DefenderState.PATROLING;
        }
    }


    // --------- WAIT (Esperar no Waypoint) ---------

    IEnumerator WaitAtWaypoint()
    {
        if (waitingAtWaypoint) yield break;
        
        waitingAtWaypoint = true;
        currentState = DefenderState.WAITING;
        
        anim.SetBool("IsChasing", false);
        agent.speed = 0f;
        
        yield return new WaitForSeconds(waitDuration);

        currentState = DefenderState.ROTATING;
        waitingAtWaypoint = false;
    }
}