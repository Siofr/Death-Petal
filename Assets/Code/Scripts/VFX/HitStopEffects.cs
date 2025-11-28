using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class HitStopEffects : MonoBehaviour
{
    public float hitStopTime = 1f;
    [Range(0,1)]public float hitStopPower = 0.1f;
    //public Material hitStopMaterial;
    //public Shader[] hitStopShaders;
    public GameObject modelParent;
    
    private EventBindings<CorrectShotEvent> _correctShotEventListener;
    private EventBindings<CorrectShotPuzzleEvent> _correctShotPuzzleEventListener;
    private EventBindings<PlayerDamageEvent> _playerDamageEventListener;
    
    private List<Renderer>  _renderers;
    //private List<Material>  _originMaterials;

    [Header("Audio Paths")]
    public EventReference onEnemyShotEventPath;

    private void Awake()
    {
       _renderers = modelParent.GetComponentsInChildren<Renderer>().ToList();
       
       //_originMaterials = new List<Material>();
    }
    

    private void OnEnable()
    {
        _correctShotEventListener = new EventBindings<CorrectShotEvent>(OnEnemyShot);
        EventBus<CorrectShotEvent>.Register(_correctShotEventListener);
        
        _correctShotPuzzleEventListener = new EventBindings<CorrectShotPuzzleEvent>(OnPuzzleShot);
        EventBus<CorrectShotPuzzleEvent>.Register(_correctShotPuzzleEventListener);

        _playerDamageEventListener = new EventBindings<PlayerDamageEvent>(OnPlayerDamage);
        EventBus<PlayerDamageEvent>.Register(_playerDamageEventListener);
    }

    private void OnDisable()
    {
        EventBus<CorrectShotEvent>.Unregister(_correctShotEventListener);
        EventBus<CorrectShotPuzzleEvent>.Unregister(_correctShotPuzzleEventListener);
        EventBus<PlayerDamageEvent>.Unregister(_playerDamageEventListener);
    }

    private void OnPuzzleShot(CorrectShotPuzzleEvent ctx)
    {
        if (ctx.weight != GetComponentInParent<Weight>())
            return;
        DoHitStopEffect();
    }

    private void OnEnemyShot(CorrectShotEvent ctx)
    {
        if (ctx.enemy != GetComponentInParent<EnemyBase>())
            return;

        RuntimeManager.PlayOneShot(onEnemyShotEventPath, transform.position);
        DoHitStopEffect();
    }

    private void OnPlayerDamage(PlayerDamageEvent ctx)
    {
        if(ctx.testPlayer != GetComponentInParent<TestPlayer>())
            return;

        RuntimeManager.PlayOneShot(onEnemyShotEventPath, transform.position);
        DoHitStopEffect();
    }
    private void DoHitStopEffect()
    {
        if (Time.timeScale != 0f)
            StartCoroutine(HitStopEffectCoroutine());
    }

    IEnumerator HitStopEffectCoroutine()
    {
        foreach (var r in _renderers)
        {
            
            r.material.SetFloat("_HitStop", 1);
            
        }
        
        Time.timeScale = hitStopPower;
        yield return new WaitForSecondsRealtime(hitStopTime);
        Time.timeScale = 1f;
        
        foreach (var r in _renderers)
        {
            r.material.SetFloat("_HitStop", 0);
        }
    }
}
