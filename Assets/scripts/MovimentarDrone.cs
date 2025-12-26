using System.Collections;
using UnityEngine;

public class MovimentarDrone : MonoBehaviour
{
    // Array com os pontos do caminho (Empty GameObjects)
    public Transform[] caminhoArray;

    // Booleano que indica se o drone terminou o voo
    public bool pararVoo = false;

    // Distância mínima para considerar que chegou ao ponto
    public float distanciaParaMudarCaminho = 0.01f;

    // Tempo que demora a percorrer o trajeto (quanto menor, mais rápido)
    public float tempoDeTrajeto = 5f;

    // Método que inicia o voo
    void Start()
    {
        StartCoroutine(VooDoDrone());
    }
    void Update()
    {
        
    }
    void OnDestroy( )
    {
        pararVoo = true;
    }

    // Coroutine que movimenta o drone de ponto a ponto
    IEnumerator VooDoDrone()
    {
        int indiceCaminho = 0;
        Vector3 posicaoDrone = gameObject.transform.position;
        float tempo = 0f;

        // Espera 1 segundo antes de iniciar
        yield return new WaitForSeconds(1f);

        // Repete até o voo terminar
        while (pararVoo == false)
        {
            // Distância entre drone e o ponto atual do caminho
            if (Vector3.Distance(gameObject.transform.position,
                                 caminhoArray[indiceCaminho].position)
                < distanciaParaMudarCaminho)
            {
                // Passa ao próximo ponto
                indiceCaminho++;
                indiceCaminho = indiceCaminho % caminhoArray.Length;
                gameObject.transform.position = caminhoArray[indiceCaminho].position;
                tempo = 0;
            }
            else
            {
                // Move gradualmente entre o ponto atual e o próximo
                gameObject.transform.position =
                    Vector3.Lerp(posicaoDrone,
                                 caminhoArray[indiceCaminho].position,
                                 tempo);

                tempo += Time.deltaTime / tempoDeTrajeto;
            }

            yield return null;
        }
    }
}
