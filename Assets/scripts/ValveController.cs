using UnityEngine;

public class ValveController : MonoBehaviour
{
    private bool jaAtivada = false;
    private GameManager gameManager;

    void Start()
    {
        // Usa o método atualizado para encontrar o Manager
        gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager não encontrado para as válvulas!");
        }
    }

    // NOVA LÓGICA: Ativação por Colisão (Encostar)
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se quem encostou foi o jogador (Assumindo Tag "Player")
        if (other.CompareTag("Player"))
        {
            if (jaAtivada) return; // Não permite rodar duas vezes

            jaAtivada = true;

            // 1. Feedback Visual: Faz a válvula rodar 
            transform.Rotate(0, 0, 90);

            // 2. Notificar o GameManager para contar como objetivo
            if (gameManager != null)
            {
                gameManager.AtivarValvula();
            }

            // Desabilitar o Collider para evitar que o jogador ative a válvula várias vezes
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }
}