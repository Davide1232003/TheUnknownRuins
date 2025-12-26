using UnityEngine;
using UnityEngine.SceneManagement;

// A classe deve ter o mesmo nome do ficheiro: Pausa_Menu
public class Pausa_Menu : MonoBehaviour
{
    // Ligar este campo no Inspector ao GameObject 'Menu Pausa'
    [Tooltip("O GameObject do Menu de Pausa completo (GrupoPausaMenu).")]
    public GameObject pauseMenuUI;

    // Estado atual do jogo
    private bool isPaused = false;

    void Start()
    {
        // Garante que o menu está escondido no início
        pauseMenuUI.SetActive(false);
        // Garante que o tempo do jogo está a correr normalmente
        Time.timeScale = 1f; 
        isPaused = false;
    }

    void Update()
    {
        // Deteta a tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // --- Métodos de Controlo de Pausa/Despausa ---

    public void PauseGame()
    {
        // 1. Mostrar o menu
        pauseMenuUI.SetActive(true);
        
        // 2. Parar o tempo do jogo (velocidade normal é 1f)
        Time.timeScale = 0f; 
        
        // 3. Informar o script que o jogo está pausado
        isPaused = true;
        
        // 4. Desbloquear o cursor (essencial para clicar nos botões)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        // 1. Esconder o menu
        pauseMenuUI.SetActive(false);
        
        // 2. Restaurar o tempo do jogo
        Time.timeScale = 1f;
        
        // 3. Informar o script que o jogo está a correr
        isPaused = false;

        // 4. Bloquear o cursor novamente para o gameplay
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }


    // --- Métodos dos Botões (Ligados no Inspector) ---

    public void LoadSettings()
    {
        Debug.Log("Abrir Definições...");
        // Adicione aqui a lógica para abrir o painel de definições
    }

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
}