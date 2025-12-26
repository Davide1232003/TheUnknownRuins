using UnityEngine;

public class MuzzleFlashNPC : MonoBehaviour
{
    [Header("Ponto onde sai o efeito")]
    public Transform pontoSaida;

    [Header("Prefab do efeito (Particle System)")]
    public GameObject muzzleFlashPrefab;

    [Header("Tempo que o efeito dura (segundos)")]
    public float duracao = 0.1f;

    public void AtivarMuzzleFlash()
    {
        if (muzzleFlashPrefab != null && pontoSaida != null)
        {
            GameObject efeito = Instantiate(muzzleFlashPrefab, pontoSaida.position, pontoSaida.rotation);
            Destroy(efeito, duracao);
        }
    }
}