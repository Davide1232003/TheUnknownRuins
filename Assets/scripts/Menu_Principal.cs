using UnityEngine;
using UnityEngine.SceneManagement; 

public class Menu_Principal : MonoBehaviour
{
    // REFERÊNCIAS DE PAINÉIS
    // -----------------------------------------------------------
    public GameObject MainMenuPanel;    // O seu painel principal de botões
    public GameObject HowToPlayPanel;   // Painel "How to Play"
    public GameObject ExitGamePanel;    // <<--- NOVO: O seu "Painel_ExitGame"
    // -----------------------------------------------------------


    // --- NAVEGAÇÃO "HOW TO PLAY" ---
    public void OpenHowToPlay()
    {
        MainMenuPanel.SetActive(false); 
        HowToPlayPanel.SetActive(true); 
    }

    public void CloseHowToPlay()
    {
        HowToPlayPanel.SetActive(false); 
        MainMenuPanel.SetActive(true); 
    }


    // --- NAVEGAÇÃO "EXIT GAME" (NOVO) ---
    
    // Ligue esta função ao botão "EXIT GAME" do Menu Principal
    public void OpenExitConfirmation()
    {
        MainMenuPanel.SetActive(false); // Esconde o menu principal
        ExitGamePanel.SetActive(true);  // Mostra o painel de confirmação
    }

    // Ligue esta função ao botão "BACK" do Painel de Saída
    public void CloseExitConfirmation()
    {
        ExitGamePanel.SetActive(false); // Esconde o painel de confirmação
        MainMenuPanel.SetActive(true);  // Volta ao menu principal
    }


    // --- AÇÕES DO JOGO ---

    public void StartGame()
    {
        SceneManager.LoadScene("Ambientedearvores");
        Time.timeScale = 1f; 
    }

    // Ligue esta função ao botão vermelho "SAIR" (dentro do Painel_ExitGame)
    // ATENÇÃO: Esta função AGORA só deve ser chamada pelo botão final de confirmar.
    public void QuitGame()
    {
        Debug.Log("A Sair do Jogo...");
        
        // Se estiver no Editor da Unity
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        // Se for um build (jogo compilado)
        #else
            Application.Quit();
        #endif
    }

    // Mantenho a função antiga vazia ou redirecionada para evitar erros se esqueceu de desligar algo,
    // mas o ideal é usar as novas acima.
    public void OpenSettings()
    {
        Debug.Log("Abrir Menu de Opções...");
    }
}