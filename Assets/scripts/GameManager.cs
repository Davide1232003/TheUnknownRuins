using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Configuração Geral")]
    public string nextSceneName = "NomeDaProximaCena";
    private PortalController portalController;

    // --- Lógica Nível 1: Gemas ---
    private int totalGems = 0;
    private int collectedGems = 0;

    // --- Lógica Nível 2: Chaves e Válvulas ---
    // NOVO: Chaves Totais agora é o objetivo
    [Header("Objetivos Nível 2 (Chaves & Válvulas)")]
    public int chavesTotais = 2;
    private int chavesColetadas = 0;

    public int valvulasTotais = 3; // Definir 3 no Inspector para Nível 2
    private int valvulasAbertas = 0;

    void Start()
    {
        // 1. Configurar Portal
        portalController = FindObjectOfType<PortalController>();
        if (portalController == null) Debug.LogError("PortalController não encontrado!");
        else portalController.gameObject.SetActive(false);

        // 2. Contagem Automática de Gemas (Para o Nível 1)
        totalGems = GameObject.FindGameObjectsWithTag("GEM").Length;

        // Logs de Debug para saber em que modo estamos
        if (totalGems > 0)
        {
            Debug.Log($"MODO NÍVEL 1: Detetadas {totalGems} Gemas.");
        }
        else if (valvulasTotais > 0 || chavesTotais > 0)
        {
            Debug.Log($"MODO NÍVEL 2: Total de {chavesTotais} Chaves e {valvulasTotais} Válvulas.");
        }
    }

    // ------------------------------------------
    // Lógica Nível 1 (Gemas)
    // ------------------------------------------
    public void CollectGem()
    {
        collectedGems++;
        Debug.Log($"Gema: {collectedGems}/{totalGems}");

        if (totalGems > 0 && collectedGems >= totalGems)
        {
            AtivarFinalDoNivel();
        }
    }

    // ------------------------------------------
    // Lógica Nível 2 (Chaves e Válvulas)
    // ------------------------------------------

    // Chamado pelo script da Chave
    public void ApanharChave()
    {
        chavesColetadas++;
        Debug.Log($"Chave: {chavesColetadas}/{chavesTotais}");

        // Verifica a condição final após apanhar a chave
        VerificarCondicaoDeVitoria();
    }

    // Chamado pelo script da Válvula
    public void AtivarValvula()
    {
        valvulasAbertas++;
        Debug.Log($"Válvula: {valvulasAbertas}/{valvulasTotais}");

        // Verifica a condição final após ativar a válvula
        VerificarCondicaoDeVitoria();
    }

    // ------------------------------------------
    // Funções Comuns
    // ------------------------------------------

    // NOVO: Função centralizada para verificar a vitória
    private void VerificarCondicaoDeVitoria()
    {
        // Só executa a lógica de válvulas/chaves se houver objetivos definidos
        if (valvulasTotais > 0 || chavesTotais > 0)
        {
            bool chavesCompletas = chavesColetadas >= chavesTotais;
            bool valvulasCompletas = valvulasAbertas >= valvulasTotais;

            // Condição final: Atingiu ambos os objetivos E não é o modo Gemas (que já teria ativado)
            if (chavesCompletas && valvulasCompletas)
            {
                AtivarFinalDoNivel();
            }
        }
    }

    private void AtivarFinalDoNivel()
    {
        Debug.Log("Objetivos cumpridos! Portal Ativado.");
        if (portalController != null)
        {
            portalController.ActivatePortal();
        }
    }

    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogError("Nome da próxima cena não definido!");
    }
}