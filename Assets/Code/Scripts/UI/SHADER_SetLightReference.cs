using System;
using UnityEngine;

[ExecuteInEditMode]
public class SHADER_SetLightReference : MonoBehaviour
{
    [SerializeField] private Material[] setMaterials;
    [SerializeField] private Transform referencePoint;

    private void Start()
    {
        foreach (Material material in setMaterials)
        {
            material.SetVector("_lightReference", referencePoint.localPosition);
        }
    }

    void Update()
    {
        foreach (Material material in setMaterials)
        {
            material.SetVector("_lightReference", referencePoint.localPosition);
        }
    }
}
