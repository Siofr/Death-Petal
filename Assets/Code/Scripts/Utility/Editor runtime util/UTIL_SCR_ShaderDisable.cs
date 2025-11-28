using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class UTIL_SCR_ShaderDisable : MonoBehaviour
{
    [SerializeField] private Material[] _editorHinderedMaterials;

    private void Start()
    {
        if(Application.isPlaying)
            gameObject.SetActive(false);
    }


    void Update()
    {
        float isEditor = (Application.isEditor) ? 1: 0;
        
        foreach (var editorHinderedMaterial in _editorHinderedMaterials)
        {
            editorHinderedMaterial.SetFloat("_IsEditor", isEditor);
        }
    }

    private void OnDisable()
    {
        foreach (var editorHinderedMaterial in _editorHinderedMaterials)
        {
            editorHinderedMaterial.SetFloat("_IsEditor", 0f);
        }
    }
}
