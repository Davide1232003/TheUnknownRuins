using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement; 

// A classe foi renomeada para PortalController para que o GameManager a encontre
public class PortalController : MonoBehaviour
{
    // --- Configuração do Portal (Existente) ---
    [Header("Applied to the effects at start")]
    [SerializeField] private Color portalEffectColor;

    [Header("Changing these might `break` the effects")]
    [Space(20)]
    [SerializeField] private Renderer portalRenderer;
    [SerializeField] private ParticleSystem[] effectsPartSystems;
    [SerializeField] private Light portalLight;
    [SerializeField] private Transform symbolTF;
    [SerializeField] private AudioSource portalAudio, flashAudio;

    private bool portalActive, inTransition;
    private float transitionF, lightF;
    private Material portalMat, portalEffectMat;
    private Vector3 symbolStartPos;

    private Coroutine transitionCor, symbolMovementCor;

    // --- NOVO: Referência e Variável de Estado ---
    private GameManager gameManager;
    private bool playerCanExit = false;


    private void OnEnable()
    {
        // Encontra o GameManager para lhe dizer para carregar o nível
        gameManager = FindObjectOfType<GameManager>();

       
        Material[] mats = portalRenderer.materials.ToArray();
        portalMat = mats[0];
        portalEffectMat = mats[1];

        portalMat.SetColor("_EmissionColor", portalEffectColor);
        portalMat.SetFloat("_EmissionStrength", 0);
        portalEffectMat.SetColor("_ColorMain", portalEffectColor);
        portalEffectMat.SetFloat("_PortalFade", 0f);

        symbolStartPos = symbolTF.localPosition;
        symbolTF.GetComponent<Renderer>().material = portalMat;

        //get and set light intensity
        portalLight.color = portalEffectColor;
        lightF = portalLight.intensity;
        portalLight.intensity = 0;

        foreach (ParticleSystem part in effectsPartSystems)
        {
            ParticleSystem.MainModule mod = part.main;
            mod.startColor = portalEffectColor;
        }
    }

    
    public void ActivatePortal()
    {
        // NOVO PASSO: LIGAR O GAME OBJECT PRINCIPAL PRIMEIRO!
        // Isto garante que o script está ativo antes de tentar iniciar as coroutines.
        gameObject.SetActive(true);

        // Chama a função interna para ativar o portal (true)
        F_TogglePortalGate(true);
        playerCanExit = true;
    }

    // Lógica para ativar/desativar o portal (Função Original)
    public void F_TogglePortalGate(bool _activate)
    {
        if (inTransition || portalActive == _activate)
            return;

        portalActive = _activate;

        if (_activate)//activate
        {
            foreach (ParticleSystem part in effectsPartSystems)
            {
                part.Play();
            }

            portalAudio.Play();
            flashAudio.Play();

            symbolMovementCor = StartCoroutine(SymbolMovement());
        }
        else if (!_activate)//deactivate
        {
            foreach (ParticleSystem part in effectsPartSystems)
            {
                part.Stop();
            }
        }

        if (!inTransition)
            transitionCor = StartCoroutine(PortalTransition());
    }

    // NOVO: Lógica de Transição de Nível (Colisão)
    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o portal está ativo E se foi o jogador a entrar
        // Assumindo que o jogador tem um CharacterController para a transição
        if (playerCanExit && other.GetComponent<CharacterController>() != null)
        {
            Debug.Log("Portal atravessado. A carregar o próximo nível...");
            if (gameManager != null)
            {
                // Chama a função do GameManager para carregar o nível
                gameManager.LoadNextLevel();
            }
            else
            {
                Debug.LogError("GameManager não encontrado no portal!");
            }
        }
    }

    // --- Coroutines (Resto do código do asset... Igual ao original) ---

    IEnumerator PortalTransition()
    {
        inTransition = true;

        if (portalActive)//fade in
        {
            while (transitionF < 1f)
            {
                transitionF = Mathf.MoveTowards(transitionF, 1, Time.deltaTime * 0.2f);

                portalMat.SetFloat("_EmissionStrength", transitionF);
                portalEffectMat.SetFloat("_PortalFade", transitionF * 0.4f);
                portalLight.intensity = lightF * transitionF;
                portalAudio.volume = transitionF * 0.8f;//max volume

                yield return new WaitForSeconds(Time.deltaTime);
            }

            inTransition = false;
            StopCoroutine(transitionCor);
        }
        else if (!portalActive)//fade out
        {
            while (transitionF > 0f)
            {
                transitionF = Mathf.MoveTowards(transitionF, 0f, Time.deltaTime * 0.4f);

                portalMat.SetFloat("_EmissionStrength", transitionF);
                portalEffectMat.SetFloat("_PortalFade", transitionF * 0.4f);
                portalLight.intensity = lightF * transitionF;
                portalAudio.volume = transitionF * 0.8f;//max volume

                yield return new WaitForSeconds(Time.deltaTime);
            }

            portalAudio.Stop();
            inTransition = false;
            StopCoroutine(symbolMovementCor);
            StopCoroutine(transitionCor);
        }
    }

    private IEnumerator SymbolMovement()
    {
        Vector3 randomPos = symbolStartPos;
        float lerpF = 0;

        while (true)
        {
            if (symbolTF.localPosition == randomPos)
            {
                randomPos[1] = Random.Range(-0.08f, 0.08f);
                randomPos[2] = Random.Range(-0.08f, 0.08f);

                randomPos = symbolStartPos + randomPos;
                lerpF = 0f;
            }
            else
            {
                symbolTF.localPosition = Vector3.Slerp(symbolTF.localPosition, randomPos, lerpF);
                lerpF += 0.001f;
            }

            yield return new WaitForSeconds(0.04f);
        }
    }
}