using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyMaterialController : MonoBehaviour
{

    [SerializeField]
    private Renderer[] animatedRenderers;
    [SerializeField] private bool isHellspawn;

    [SerializeField] private GameObject parent;

    public void Disintegrate()
    {
        StartCoroutine(AnimateDisintegration());
    }

    private void Start()
    {
        // Hellspawn Specific
        if(!isHellspawn) return;
        foreach (Renderer animatedRenderer in animatedRenderers)
        {
            animatedRenderer.material.SetFloat("_Face", Random.Range(0,3));
        }
    }

    private IEnumerator AnimateDisintegration()
    {
        for (float i = 0; i <= 1.1f; i += Time.deltaTime)
        {
            foreach (var animatedRenderer in animatedRenderers)
            {
                animatedRenderer.material.SetFloat("_Decintegration", i);
                animatedRenderer.material.SetFloat("_Alpha", 1-i);
            }

            yield return new WaitForFixedUpdate();
        }
        
        Destroy(parent);
    }
}
