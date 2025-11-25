using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HitFeedback : MonoBehaviour
{
    [Range(0f, 1f)]
    public float intensity;
    [Range(0f, 5f)]
    public float duration;

    private Coroutine _hitEffect = null;

    public VolumeProfile globalProfile;

    private Vignette _vignette;
    
    private EventBindings<PlayerDamagedEvent> _playerDamagedEventListener;
    
    private void Awake()
    {
        globalProfile.TryGet<Vignette>(out _vignette);
    }

    private void OnEnable()
    {
        _playerDamagedEventListener = new EventBindings<PlayerDamagedEvent>(OnEnemyHit);
        
        EventBus<PlayerDamagedEvent>.Register(_playerDamagedEventListener);
    }

    private void OnDisable()
    {
        EventBus<PlayerDamagedEvent>.Unregister(_playerDamagedEventListener);
    }
    
    public void OnEnemyHit()
    {
        print(_hitEffect);

        if (_hitEffect == null)
            _hitEffect = StartCoroutine(HitEffect());
    }

    private IEnumerator HitEffect()
    {
        print("entered");

        _vignette.active = true;
        _vignette.intensity.value = 0;

        float lerp = 0f;
        
        while(lerp < 1f)
        {
            lerp += Time.deltaTime / duration * 2;
            _vignette.intensity.value = Mathf.Lerp(0f, intensity, lerp);
            yield return null;
        }

        lerp = 0f;
        _vignette.intensity.value = intensity;

        while (lerp < 1f)
        {
            lerp += Time.deltaTime / duration * 2;
            _vignette.intensity.value = Mathf.Lerp(intensity, 0f, lerp);
            yield return null;
        }

        _vignette.intensity.value = 0f;
        print("yurr");

        _vignette.active = false;
        _vignette.intensity.value = 0f;

        _hitEffect = null;
    }
}