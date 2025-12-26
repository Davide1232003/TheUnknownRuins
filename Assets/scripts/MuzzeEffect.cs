using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MuzzeEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine("DestroyObj");
    }

    // Update is called once per frame
    IEnumerator DestroyObj()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}