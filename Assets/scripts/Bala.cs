using UnityEngine;

public class Bala : MonoBehaviour
{
    [Header("Configuração da Bala")]
    public float speed = 50f;
    public float lifetime = 3f; // Destruir a bala após 3 segundos (para limpeza)
    
    // NOTA: Removemos a lógica de dano por enquanto, focando apenas no movimento.

    void Start()
    {
        // Destruir o projétil após um tempo
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Movimento constante para a frente (no eixo Z local)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    
    // Opcional: Para evitar que a bala atravesse o mundo (requer um collider no mundo)
    void OnTriggerEnter(Collider other)
    {
        // A bala atinge algo e é destruída
        Destroy(gameObject);
    }
}