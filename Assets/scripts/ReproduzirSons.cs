using UnityEngine;

public class ReproduzirSons : MonoBehaviour
{
    public AudioSource sommovimentoCabeca;
    public AudioSource somMovimentoPernas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ReproduzirSomCabecaCabec()
    {
        sommovimentoCabeca.Play();
    }
    public void ReproduzirSomPernas()
    {
        somMovimentoPernas.Play();
    }
}
