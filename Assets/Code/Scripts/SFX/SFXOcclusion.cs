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
    private AmbientSFXManager _ambManager;
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
        _ambManager = GetComponentInParent<AmbientSFXManager>();
    }

    private void OnEnable()
    {
        EventBus<CameraChangeEvent>.Register(_cameraChangeEventListener);
    }

    void OnListenerPositionChange(CameraChangeEvent ctx)
    {
        if (!CheckDistanceToListener(ctx.transform.position)) return;
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

    public float GetOcclusionValue(float distance)
    {
        float difference = maxDistance - distance;
        float distanceMult = _distanceWeighting * (difference / maxDistance);

        return _occlusionWeighting + distanceMult;

        // eventInstance.setParameterByID(_occlusionParam, param);
    }
}
