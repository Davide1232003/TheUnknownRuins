using UnityEngine;
using UnityEngine.InputSystem;

// Nota: A definição da interface IInteractable deve estar num ficheiro separado.
// public interface IInteractable { void Interact(); } 

public class MovimentarJogador : MonoBehaviour
{
    // --- Variáveis de Rotação e Sensibilidade ---
    public float sensibilidadeX = 1f;
    public float sensibilidadeY = 1f;
    public GameObject objetoCamera;
    private Vector2 vetorRotacao;

    // --- Variáveis de Movimento ---
    public float velocidadeMovimento = 4f;
    public float multiplicadorCorrida = 2f;
    private Vector2 inputMovimento;
    private Vector3 movimento;
    private CharacterController characterController;

    // --- Variáveis de Salto e Gravidade ---
    public float efeitoGravidade = 1f;
    public float forcaSalto = 5f;
    private int saltosDisponiveis = 2;

    // --- Variáveis de Agachar (Crouch) ---
    private float originalHeight;
    public float crouchHeight = 1.0f;
    public float crouchSpeedMultiplier = 0.5f;
    private bool isCrouching = false;

    // --- Variáveis de Interação e Coleção ---
    public float distanciaInteracao = 3f;
    public LayerMask layerInteracao;

    // NOVO: Ligação pública forçada (Atribuído no Inspector)
    public GameManager gameManager;

    void Start()
    {
        // Configuração Inicial da Câmara e Controlo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;

        // Verifica se a ligação ao GameManager foi feita no Inspector
        if (gameManager == null)
        {
            Debug.LogError("GameManager NÃO ATRIBUÍDO! Por favor, arraste o objeto GameManager para o Inspector.");
        }
    }

    void Update()
    {
        // Rotação da Câmera
        transform.localRotation = Quaternion.Euler(0, vetorRotacao.x, 0);
        objetoCamera.transform.localRotation = Quaternion.Euler(-vetorRotacao.y, 0, 0);

        // Movimento
        Vector3 direcao = transform.right * inputMovimento.x + transform.forward * inputMovimento.y;
        float velocidadeFinal = velocidadeMovimento;

        // Aplica a velocidade de corrida ou de agachar
        if (Keyboard.current.leftShiftKey.isPressed && !isCrouching)
        {
            velocidadeFinal *= multiplicadorCorrida;
        }
        else if (isCrouching)
        {
            velocidadeFinal *= crouchSpeedMultiplier;
        }

        movimento.x = direcao.x * velocidadeFinal;
        movimento.z = direcao.z * velocidadeFinal;

        // Gravidade e Salto Duplo
        if (characterController.isGrounded)
        {
            if (movimento.y < 0)
                movimento.y = -0.5f;
            saltosDisponiveis = 2;
        }
        else
        {
            movimento.y += Physics.gravity.y * efeitoGravidade * Time.deltaTime;
        }

        characterController.Move(movimento * Time.deltaTime);
    }

    // ------------------------------------------------------------------
    // Métodos de Input System
    // ------------------------------------------------------------------

    public void OnLook(InputValue context)
    {
        Vector2 inputLook = context.Get<Vector2>();
        vetorRotacao.x += inputLook.x * sensibilidadeX;
        vetorRotacao.y = Mathf.Clamp(vetorRotacao.y + inputLook.y * sensibilidadeY, -90f, 90f);
    }

    void OnMove(InputValue value)
    {
        inputMovimento = value.Get<Vector2>();
    }

    public void OnJump(InputValue context)
    {
        if (context.isPressed)
        {
            if (characterController.isGrounded)
            {
                movimento.y = forcaSalto;
                saltosDisponiveis = 1;
            }
            else if (saltosDisponiveis > 0)
            {
                movimento.y = forcaSalto;
                saltosDisponiveis--;
            }
        }
    }

    public void OnCrouch(InputValue context)
    {
        if (context.isPressed)
        {
            isCrouching = !isCrouching;

            if (isCrouching)
            {
                characterController.height = crouchHeight;
                objetoCamera.transform.localPosition = new Vector3(
                    objetoCamera.transform.localPosition.x,
                    crouchHeight * 0.9f,
                    objetoCamera.transform.localPosition.z);
            }
            else
            {
                characterController.height = originalHeight;
                objetoCamera.transform.localPosition = new Vector3(
                    objetoCamera.transform.localPosition.x,
                    originalHeight * 0.9f,
                    objetoCamera.transform.localPosition.z);
            }
        }
    }

    public void OnInteract(InputValue context)
    {
        if (!context.isPressed) return;

        Ray ray = new Ray(objetoCamera.transform.position, objetoCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distanciaInteracao, layerInteracao))
        {
            Debug.Log("Interagiu com: " + hit.collider.gameObject.name);

            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }


    // ------------------------------------------------------------------
    // Lógica de Coleção (Gemas e Chaves) - FINAL
    // ------------------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (gameManager == null) return;

        // 1. Lógica do Nível 1: Apanhar Gemas
        if (other.CompareTag("GEM"))
        {
            gameManager.CollectGem();
            Destroy(other.gameObject);
        }

        // 2. Lógica do Nível 2: Apanhar Chaves
        if (other.CompareTag("KEY"))
        {
            gameManager.ApanharChave();
            Destroy(other.gameObject);
        }
    }
}