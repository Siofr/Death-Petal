using UnityEngine;
using FMODUnity;
using SFXUtil;
using FMOD.Studio;

public class SFXOcclusion : MonoBehaviour
{
    public float maxDistance;
    public EventReference eventPath;
    private EventInstance eventInstance;

    private EventBindings<CameraChangeEvent> _cameraChangeEventListener;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            // Stop playing the audio and return
            return;
        }

        if(!CheckOcclusion(ctx.transform.position));
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

    void ChangeAmbientValues()
    {

    }
}
