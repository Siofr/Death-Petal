using System.Collections;
using UnityEngine;

public class ShineSphere : MonoBehaviour
{
    private Material _material;
    
    public float shineTime = 0.5f;
    void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
        
        GetComponent<MeshRenderer>().material = _material;
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(ShineFrames());
    }

    IEnumerator ShineFrames()
    {
        float timer = 0f;
        while (timer < shineTime)
        {
            timer += Time.fixedDeltaTime;
            _material.SetFloat("_UnityTime", timer);
            
            yield return new WaitForFixedUpdate();
        }
        
        _material.SetFloat("_UnityTime", shineTime);
        
        //print("shine finished");
        gameObject.SetActive(false);
    }
}
