using UnityEngine;
using UnityEngine.Events;

public class Saude_Npc : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Animation Settings")]
    [Tooltip("Arraste o componente Animator do NPC para cá")]
    public Animator animator;
    [Tooltip("Nome do Trigger no Animator (Ex: Morrer)")]
    public string parametroMorte = "Morrer";
    [Tooltip("Tempo em segundos para destruir o objeto após a morte (duração da animação)")]
    public float tempoParaDestruir = 3.0f;

    [Header("Events")]
    public UnityEvent OnDie;

    private bool isDead = false; // Evita chamar a morte mais de uma vez

    void Awake()
    {
        currentHealth = maxHealth;
        
        // Tenta pegar o Animator automaticamente se não tiver sido atribuído manualmente
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return; // Se já morreu, não faz nada

        currentHealth -= amount;
        Debug.Log(gameObject.name + " recebeu dano. Vida: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true; // Marca como morto
        
        // 1. Toca a animação
        if (animator != null)
        {
            Debug.Log("Dead");
            animator.SetTrigger(parametroMorte);
        }
        else
        {
            Debug.LogWarning("Animator não atribuído no Saude_Npc!");
        }

        // 2. Desativa o Collider e a Física (Importante!)
        // Isso impede que o jogador continue atirando no cadáver ou colidindo com ele
        var collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;
        
        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true; // Impede que a física afete o corpo caindo

        // 3. Avisa outros scripts (Unity Event)
       // OnDie.Invoke();

        Debug.Log(gameObject.name + " morreu. Destruindo em " + tempoParaDestruir + " segundos...");

        // 4. Agenda a destruição para daqui a X segundos
       Destroy(gameObject, tempoParaDestruir);
    }

    public bool IsAlive()
    {
        return !isDead;
    }
}