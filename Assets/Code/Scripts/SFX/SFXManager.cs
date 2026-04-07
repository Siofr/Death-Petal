using UnityEngine;
using FMOD.Studio;
using FMODUnity;

/* Example of creating and playing an FMOD event that needs to be repeated or with parameters that need to be changed:
 * 
 *  public EventReference shootSfxEventPath; (A reference to the fmod path, set this in inspector)
 *  private EventInstance _shootEvent;
 *  private PARAMETER_ID _bulletsLeft; (We create a parameter ID for each fmod parameter
 * 
 * void Start()
 * {
 *      _bulletsLeft = SFXUtilities.AssignParamID("BulletLeft", shootSfxEventPath);
 *      _shootEvent = SFXUtilities.CreateEventInstance(shootSfxEventPath, this.gameObject);
 * }
 * 
 * void PlayEvent()
 * {
 *      _shootEvent.setParameterByID(_bulletsLeft, bulletIndex); (We change the fmod paramater value)
 *      EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(_shootEvent)); (We play the event)
 * }
 * 
 * void OnDestroy()
 * {
 *      _shootEvent.release();
 * }
 * 
 * If the audio doesn't require changing parameters and only plays once you can just use:
 * 
 * FMODUnity.RuntimeManager.PlayOneShot(eventReference, sourcePosition);
 * 
 */


public struct SFXEventTrigger : IEvent
{
    public EventInstance eventInstance;
    public GameObject sourceObject;

   public SFXEventTrigger(EventInstance eventInstance, GameObject sourceObject)
    {
        this.eventInstance = eventInstance;
        this.sourceObject = sourceObject;
    }
}

public struct SFXStopEvent : IEvent
{
    public EventInstance eventInstance;

    public SFXStopEvent(EventInstance eventInstance)
    {
        this.eventInstance = eventInstance;
    }
}

public class SFXManager : Singleton<SFXManager>
{
    private EventBindings<SFXEventTrigger> _sfxEventListener;
    private EventBindings<SFXStopEvent> _sfxStopEventListener;

    protected override void Awake()
    {
        base.Awake();
        _sfxEventListener = new EventBindings<SFXEventTrigger>(PlaySFX);
        _sfxStopEventListener = new EventBindings<SFXStopEvent>(StopSFX);
    }

    public void OnEnable()
    {
        EventBus<SFXEventTrigger>.Register(_sfxEventListener);
    }

    public void ChangeEventParameter(EventInstance eventInstance, PARAMETER_ID paramID, float paramValue)
    {
        eventInstance.setParameterByID(paramID, paramValue, false);
    }

    public void PlaySFX(SFXEventTrigger ctx)
    {
        RuntimeManager.AttachInstanceToGameObject(ctx.eventInstance, ctx.sourceObject);
        ctx.eventInstance.start();
        // ctx.eventInstance.release();
    }

    public void StopSFX(SFXStopEvent ctx)
    {
        ctx.eventInstance.release();
    }
}
