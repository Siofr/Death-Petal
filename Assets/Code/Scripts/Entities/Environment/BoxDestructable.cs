using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class BoxDestructible : EntityBase
{
    [Header("Box Destructible Fields")] 
    [SerializeField] private float _postDestructionLifetime = 1f;

    [SerializeField] private float _postDestructionFadeTime = 1f;

    [SerializeField] private GameObject _solidBox;
    [SerializeField] private GameObject _fragmentedBox;

    private Renderer[] _fragmentedRenderers;
    private bool _hasBeenShot = false;

    [Header("Audio Paths")]
    public EventReference onBoxShotEventPath;

    protected override void Awake()
    {
        _solidBox.SetActive(true);
        _fragmentedBox.SetActive(false);

        _fragmentedRenderers = _fragmentedBox.GetComponentsInChildren<Renderer>();
    }
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        RuntimeManager.PlayOneShot(onBoxShotEventPath, transform.position);

        _solidBox.SetActive(false);
        _fragmentedBox.SetActive(true);

        _hasBeenShot = true;
        StartCoroutine(DisposeBox());
    }

    private IEnumerator DisposeBox()
    {
        yield return new WaitForSeconds(_postDestructionLifetime);

        for (float i = 0; i <= _postDestructionFadeTime; i += Time.deltaTime)
        {
            foreach (var fragmentedRenderer in _fragmentedRenderers)
            {
                fragmentedRenderer.material.SetFloat("_Alpha", (1- i/_postDestructionLifetime));
            }

            yield return new WaitForFixedUpdate();
            
        }
        Destroy(this.gameObject);
    }
}
