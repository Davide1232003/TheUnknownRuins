using UnityEngine;

public class TesteDano : MonoBehaviour
{
    [Header("Alvo")]
    public Saude_Npc targetNPC; // Arraste o objeto "policia" para aqui no Inspector

    [Header("Teste")]
    public float damageAmount = 50f;

    void Update()
    {
        // Se premir a tecla ESPAÇO, o NPC leva 50 de dano.
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (targetNPC != null && targetNPC.IsAlive())
            {
                // Chama a função TakeDamage() no script Saude_Npc
                targetNPC.TakeDamage(damageAmount);
            }
        }

        // Se premir a tecla K, mata o NPC imediatamente (dano total)
        if (Input.GetKeyDown(KeyCode.K))
        {
             if (targetNPC != null && targetNPC.IsAlive())
            {
                targetNPC.TakeDamage(1000f); // Dano suficiente para matar
            }
        }
    }
}