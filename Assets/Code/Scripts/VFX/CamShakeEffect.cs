using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CamShakeEffect : MonoBehaviour
{
    private EventBindings<CorrectShotEvent> _correctShotEventListener;
    private EventBindings<CorrectShotPuzzleEvent> _correctShotPuzzleEventListener;
    private EventBindings<PlayerDamageEvent> _playerDamageEventListener;
    
    public float ShakeAmplitude = 1f;
    public float ShakeTime = 1f;
    private void OnEnable()
    {
        _correctShotEventListener = new EventBindings<CorrectShotEvent>(ShakeCamera);
        EventBus<CorrectShotEvent>.Register(_correctShotEventListener);
        
        _correctShotPuzzleEventListener = new EventBindings<CorrectShotPuzzleEvent>(ShakeCamera);
        EventBus<CorrectShotPuzzleEvent>.Register(_correctShotPuzzleEventListener);

        _playerDamageEventListener = new EventBindings<PlayerDamageEvent>(ShakeCamera);
        EventBus<PlayerDamageEvent>.Register(_playerDamageEventListener);
    }

    private void OnDisable()
    {
        EventBus<CorrectShotEvent>.Unregister(_correctShotEventListener);
        EventBus<CorrectShotPuzzleEvent>.Unregister(_correctShotPuzzleEventListener);
        EventBus<PlayerDamageEvent>.Unregister(_playerDamageEventListener);
    }

    private void ShakeCamera()
    {
        print("Starting Camera Shake");
        StartCoroutine("ShakingCoroutine");

    }
    
    IEnumerable ShakingCoroutine()
    {
        CinemachineBasicMultiChannelPerlin camShaker = GameObject.FindFirstObjectByType<CinemachineBasicMultiChannelPerlin>();
        
        print(camShaker.name + "is shaking");

        if (camShaker.AmplitudeGain != 0f) 
            yield return null;

        camShaker.AmplitudeGain = ShakeAmplitude;
        
        yield return new WaitForSecondsRealtime(ShakeTime);
        
        camShaker.AmplitudeGain = 0f;
    }
}
