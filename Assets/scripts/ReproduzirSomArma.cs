using UnityEngine;

public class ReproduzirSomArma : MonoBehaviour
{
    // variável que irá guardar o AudioSource do componente.
    private AudioSource somArma;

    // Start is called before the first frame update
    void Start()
    {
        // Atribui à variável somArma o componente AudioSource do que estiver no gameObject
        somArma = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Método responsável por reproduzir o som.
    public void PlaySomArma()
    {
        // Reproduz a som que estiver no AudioSource.
        somArma.Play();
    }
}
