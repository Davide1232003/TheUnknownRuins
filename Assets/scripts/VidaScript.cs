using UnityEngine;
using UnityEngine.UI;

public class VidaScript : MonoBehaviour
{
    // Variável que guarda a vida inicial que o gameObject tem.
    public int vidaInicial = 100;
    // Variável do tipo boleana que indica se o gameObject pode fazer Respawn ou não.
    public bool fazerRespawn = false;
    // Variável do tipo Vector3 que indica a posição onde irá fazer Respawn.
    private Vector3 posicaoRespawn;
    // Variável que guarda a vida que o jogador tem a cada momento.
    private int vidaAtual;
    public Image imagemVida;

    void Start()
    {
        // Quando o jogo iniciar, guarda na variável posicaoRespawn, a posição atual do gameObject.
        posicaoRespawn = gameObject.transform.position;
        // Quando o jogo iniciar, coloca a vidaAtual com o mesmo valor que a vidaInicial.
        vidaAtual = vidaInicial;
    }

    void Update()
    {
        // O método Update está vazio na imagem fornecida.
    }

    // Esta função é chamada quando uma particula collde com um collider.
    // Esta função é chamada en dois GameObjects, no que lançou a particula e no que fol atingido pela particula.
    // No caso de este Script estar no gameObject que lançou a particula, o GameObject other (parametro de entrada da função)
    // val ser o foi atingido pela particula. No caso de este Script estar no gameObject que foi atingido pela particula,
    // o Gamelbject other vai ser o emissor da particula.
    private void OnParticleCollision(GameObject other)
    {
        // Verifica se o GameObject que exitiu a particula possui o componente DanoScript.
        // Se existir, guarde a referência na variável local danoScript.
        if (other.TryGetComponent(out DanoScript danoScript))
        {
            // Reduz a vida actual con base no valor de dano definido no DanoScript.
            vidaAtual -= danoScript.dano;

            // Se a vida chegar a serro ou menos, aplica a lógica de morte ou respawn.
            if (vidaAtual <= 0)
            {
                if (fazerRespawn)
                {
                    // Move o GameObject para a posição de respawn.
                    transform.position = posicaoRespawn;
                    // Garante que o motor de fisica reconhece a nova posição.
                    Physics.SyncTransforms();
                    // Restaura a vida inicial.
                    vidaAtual = vidaInicial;
                }
                else
                {
                    // Desactiva o Gamelbject se não for para fazer respawn.
                    gameObject.SetActive(false);
                }
              
            }
            if (imagemVida != null)
            {
                Debug.LogWarning((float)vidaAtual / vidaInicial);
                imagemVida.fillAmount = (float)vidaAtual / vidaInicial;
            }
        }
    }
    public void MudarPosicaoRespawn()
    {
        posicaoRespawn = gameObject.transform.position;
    }
}
