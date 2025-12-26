using UnityEngine;
using UnityEngine.SceneManagement;

public class menuscript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NovoJogo()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void Sair()
    {
#if UNITY_EDITOR
        //Application. Quit() não funciona no editor por isso alte I valor de Unity. isPlaying para falso para interromper a
        UnityEditor.EditorApplication.isPlaying = false;
#else
Application. QuitO);
#endif
    }
}