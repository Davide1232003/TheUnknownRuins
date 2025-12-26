using UnityEngine;
using System.Collections;

public class GateController : MonoBehaviour, IInteractable
{
    private bool estaAberto = false;
    private bool emTransicao = false;

    public float openSpeed = 2f;
    public Vector3 rotationAmount = new Vector3(0, 90, 0);

    // O GameManager é que vai chamar este método
    public void AbrirPortaoAutomaticamente()
    {
        if (estaAberto || emTransicao) return;

        StartCoroutine(AbrirPortaoSuavemente());
    }

    // A função Interact é agora opcional, mas útil para mensagens de feedback
    public void Interact()
    {
        if (estaAberto) return;

        // Mensagem que a porta agora depende das chaves
        Debug.Log("Esta porta abre automaticamente quando todas as chaves forem coletadas.");
    }

    IEnumerator AbrirPortaoSuavemente()
    {
        emTransicao = true;

        // ... (código da coroutine para rotação suave é o mesmo) ...

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(transform.eulerAngles + rotationAmount);

        float tempoDecorrido = 0f;

        while (tempoDecorrido < 1f)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, tempoDecorrido);
            tempoDecorrido += Time.deltaTime * openSpeed;
            yield return null;
        }

        transform.rotation = endRotation;

        estaAberto = true;
        emTransicao = false;

        // Remove o Collider para que o jogador possa passar
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        Debug.Log("Portão Aberto Automaticamente!");
    }
}