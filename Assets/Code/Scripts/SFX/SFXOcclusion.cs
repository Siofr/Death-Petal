using UnityEngine;
using FMODUnity;
using SFXUtil;
using FMOD.Studio;

public class SFXOcclusion : MonoBehaviour
{
    public float occlusionModifier;
    public float maxDistance;
    public EventReference eventPath;
    private EventInstance eventInstance;

    private PARAMETER_ID _occlusionParam;

    private float _occlusionWeighting = 0.7f;
    private float _distanceWeighting = 0.3f;

    private bool _isPlaying = false;
    private EventBindings<CameraChangeEvent> _cameraChangeEventListener;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _occlusionParam = SFXUtilities.AssignParamID("Occlusion", eventPath);
        eventInstance = SFXUtilities.CreateEventInstance(eventPath, this.gameObject);
    }

    private void Awake()
    {
        _cameraChangeEventListener = new EventBindings<CameraChangeEvent>(OnListenerPositionChange);
    }

    private void OnEnable()
    {
        EventBus<CameraChangeEvent>.Register(_cameraChangeEventListener);
    }

    void OnListenerPositionChange(CameraChangeEvent ctx)
    {
        if (!CheckDistanceToListener(ctx.transform.position))
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _isPlaying = false;
            return;
        }

        if(CheckOcclusion(ctx.transform.position))
        {
            ChangeAmbientValues(Vector3.Distance(ctx.transform.position, transform.position));
        }
        else
        {
            eventInstance.setParameterByID(_occlusionParam, 0);
        }

        if (!_isPlaying)
        {
            eventInstance.start();
            _isPlaying = true;
        }
    }

    bool CheckDistanceToListener(Vector3 listenerPos)
    {
        return Vector3.Distance(transform.position, listenerPos) <= maxDistance;
    }

    bool CheckOcclusion(Vector3 listenerPos)
    {
        RaycastHit hit;
        Physics.Linecast(transform.position, listenerPos, out hit);

        return hit.collider;
    }

    void ChangeAmbientValues(float distance)
    {
        float difference = maxDistance - distance;
        float distanceMult = _distanceWeighting * (difference / maxDistance);

        float param = _occlusionWeighting + distanceMult;

        eventInstance.setParameterByID(_occlusionParam, param);
    }
}
