using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CamShakeEffect : MonoBehaviour
{
    private EventBindings<CorrectShotEvent> _correctShotEventListener;
    private EventBindings<CorrectShotPuzzleEvent> _correctShotPuzzleEventListener;
    private EventBindings<PlayerDamageEvent> _playerDamageEventListener;
    
    public float ShakeAmplitude = 1f;
    public float ShakeTime = 1f;
    
    private CinemachineBrain _cinemachineBrain;

    public void Start()
    {
        _cinemachineBrain = GameObject.FindAnyObjectByType<CinemachineBrain>();
    }
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
        //print("Starting Camera Shake");

        //TODO - CURRENTLY PILING UP ERRORS
        return;
        
        StartCoroutine(ShakingCoroutine());

    }

    
    
    IEnumerator ShakingCoroutine()
    {
        //print("Attempting to shake Camera Shake");

        //_cinemachineBrain.ActiveVirtualCamera;
        
        //CinemachineBasicMultiChannelPerlin camShaker = GameObject.FindFirstObjectByType<CinemachineBasicMultiChannelPerlin>();

        if (_cinemachineBrain.ActiveVirtualCamera is CinemachineCamera cam)
        {
            CinemachineBasicMultiChannelPerlin camShaker = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            
            if (camShaker.AmplitudeGain != 0f) 
                yield return null;

            camShaker.AmplitudeGain = ShakeAmplitude;
        
            //print(camShaker.name + " in " + camShaker.gameObject.transform.parent.name + " is shaking by: " + camShaker.AmplitudeGain);
        
            yield return new WaitForSecondsRealtime(ShakeTime);
        
            camShaker.AmplitudeGain = 0f;
        }
        

        
    }
}
