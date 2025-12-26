using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Encontra o GameManager e adiciona a chave
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.ApanharChave();
                // Destroi a chave (efeito visual de apanhar)
                Destroy(gameObject);
            }
        }
    }
}