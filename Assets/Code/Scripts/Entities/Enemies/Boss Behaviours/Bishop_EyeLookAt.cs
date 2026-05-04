using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bishop_EyeLookAt : MonoBehaviour
{
    private float xAdjust = 0;
    private float yAdjust = 0;

    private MeshRenderer _renderer;
    private void Start()
    {
        xAdjust = Random.Range(0f, 0.2f);
        yAdjust = Random.Range(-0.2f, 0.2f);
        _renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.time + transform.position.y) % .5 <= 0.1)
        {
            _renderer.material.SetFloat("_OffsetX", xAdjust);
            _renderer.material.SetFloat("_OffsetY", yAdjust);
        }

        xAdjust += Random.Range(-0.01f, 0.01f);
        xAdjust = Mathf.Abs(xAdjust);
        xAdjust %= 0.2f;
        
        yAdjust += Random.Range(-0.01f, 0.01f);
        yAdjust %= 0.2f;

    }
}
