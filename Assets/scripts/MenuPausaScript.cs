using UnityEngine;
using UnityEngine.InputSystem; // Adicionar para o novo Input System

public class MenuPausaScript : MonoBehaviour
{
    // Variável para arrastar a Ação 'Pausa' no Inspector (ex: sua tecla ESC)
    public InputActionReference pauseAction;

    // Variável para arrastar o Painel/Canvas do menu
    public GameObject menuPausaUI;

    public static bool JogoEstaPausado = false;

    // Usar OnEnable/OnDisable para gerir a subscrição da ação
    private void OnEnable()
    {
        if (pauseAction != null && pauseAction.action != null)
        {
            // Liga o método OnPause à ação
            pauseAction.action.performed += OnPause;
            pauseAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null && pauseAction.action != null)
        {
            // Desliga o método para evitar erros
            pauseAction.action.performed -= OnPause;
            pauseAction.action.Disable();
        }
    }

    // Este é o método que substitui o código no Update()
    private void OnPause(InputAction.CallbackContext context)
    {
        if (JogoEstaPausado)
        {
            Continuar();
        }
        else
        {
            Pausar();
        }
    }

    // ... (Os métodos Continuar() e Pausar() permanecem os mesmos)

    public void Continuar()
    {
        menuPausaUI.SetActive(false);
        Time.timeScale = 1f;
        JogoEstaPausado = false;
    }

    void Pausar()
    {
        menuPausaUI.SetActive(true);
        Time.timeScale = 0f;
        JogoEstaPausado = true;
    }
}