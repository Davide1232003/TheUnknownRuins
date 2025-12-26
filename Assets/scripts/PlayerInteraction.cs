using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        // Encontra o Game Manager na cena
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager não encontrado na cena!");
        }
    }

    // Função que é chamada quando o Collider entra noutro Collider (marcado como Is Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // 1. Verificar se o objeto colidido é uma Gema
        if (other.CompareTag("GEM"))
        {
            // Chamar a função no Game Manager para registar a gema
            gameManager.CollectGem();

            // Destruir o objeto Gema para que desapareça
            Destroy(other.gameObject);
        }
    }
}