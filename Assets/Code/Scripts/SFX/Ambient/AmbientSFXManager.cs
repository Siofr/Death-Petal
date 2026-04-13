using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using SFXUtil;

public class AmbientSFXManager : MonoBehaviour
{
    public EventReference insideEventPath;
    private EventInstance _insideEventInstance;

    private PARAMETER_ID _outsideOcclusionParam;
    private PARAMETER_ID _insideStageParam;

    private EventBindings<CameraChangeEvent> _cameraChangeEventListener;

    private void Awake()
    {
        _cameraChangeEventListener = new EventBindings<CameraChangeEvent>(OnListenerChange);
        _outsideOcclusionParam = SFXUtilities.AssignParamID("Occlusion", insideEventPath);
        _insideEventInstance = SFXUtilities.CreateEventInstance(insideEventPath, this.gameObject);
    }

    private void OnEnable()
    {
        EventBus<CameraChangeEvent>.Register(_cameraChangeEventListener);
    }

    private void OnDisable()
    {
        EventBus<CameraChangeEvent>.Unregister(_cameraChangeEventListener);
    }

    private void Start()
    {
        _insideEventInstance.start();
    }

    public void OnListenerChange(CameraChangeEvent ctx)
    {
        EventBus<SFXSnapshot>.Raise(new SFXSnapshot(0, ctx.reverbArea));
        _insideEventInstance.setParameterByID(_outsideOcclusionParam, ctx.occlusionValue);
        _insideEventInstance.setParameterByID(_insideStageParam, ctx.ambientStage);
    }
}
