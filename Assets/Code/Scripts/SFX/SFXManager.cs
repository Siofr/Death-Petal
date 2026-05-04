using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using SFXUtil;

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

public struct SFXSnapshot : IEvent
{
    public int snapshot;
    public bool activate;

    public SFXSnapshot(int snapshot, bool activate)
    {
        this.snapshot = snapshot;
        this.activate = activate;
    }
}

public class SFXManager : Singleton<SFXManager>
{
    private EventBindings<SFXEventTrigger> _sfxEventListener;
    private EventBindings<SFXStopEvent> _sfxStopEventListener;
    private EventBindings<SFXSnapshot> _sfxSnapshotListener;
    private EventBindings<PauseEvent> _pauseEventListener;

    public List<EventReference> snapshotReferences = new List<EventReference>();
    private List<EventInstance> snapshotInstances = new List<EventInstance>();

    protected override void Awake()
    {
        base.Awake();

        _sfxEventListener = new EventBindings<SFXEventTrigger>(PlaySFX);
        _sfxStopEventListener = new EventBindings<SFXStopEvent>(StopSFX);

        foreach (EventReference snapshot in snapshotReferences)
        {
            snapshotInstances.Add(SFXUtilities.CreateEventInstance(snapshot, this.gameObject));
        }

        _sfxSnapshotListener = new EventBindings<SFXSnapshot>(ChangeSnapshot);
        _pauseEventListener = new EventBindings<PauseEvent>(OnPauseEvent);

        foreach (EventInstance snapshot in snapshotInstances)
        {
            snapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void OnEnable()
    {
        EventBus<SFXEventTrigger>.Register(_sfxEventListener);
        EventBus<SFXStopEvent>.Register(_sfxStopEventListener);
        EventBus<SFXSnapshot>.Register(_sfxSnapshotListener);
        EventBus<PauseEvent>.Register(_pauseEventListener);
    }

    public void OnDisable()
    {
        EventBus<SFXEventTrigger>.Unregister(_sfxEventListener);
        EventBus<SFXStopEvent>.Unregister(_sfxStopEventListener);
        EventBus<SFXSnapshot>.Unregister(_sfxSnapshotListener);
        EventBus<PauseEvent>.Unregister(_pauseEventListener);
    }

    public void InitializeSnapshots()
    {

    }

    public void ChangeSnapshot(SFXSnapshot ctx)
    {
        Debug.Log("Pause Called if this is 2: " + ctx.snapshot);
        if (ctx.snapshot >= snapshotInstances.Count || ctx.snapshot < 0) return;

        if (ctx.activate && !IsPlaying(snapshotInstances[ctx.snapshot]))
        {
            snapshotInstances[ctx.snapshot].start();
            snapshotInstances[ctx.snapshot].release();
        }
        else
        {
            snapshotInstances[ctx.snapshot].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void ChangeEventParameter(EventInstance eventInstance, PARAMETER_ID paramID, float paramValue)
    {
        eventInstance.setParameterByID(paramID, paramValue, false);
    }

    public void PlaySFX(SFXEventTrigger ctx)
    {
        RuntimeManager.AttachInstanceToGameObject(ctx.eventInstance, ctx.sourceObject);
        ctx.eventInstance.start();
        ctx.eventInstance.release();
    }

    public void StopSFX(SFXStopEvent ctx)
    {
        ctx.eventInstance.release();
    }

    bool IsPlaying(FMOD.Studio.EventInstance instance)
    {
        FMOD.Studio.PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        return state != FMOD.Studio.PLAYBACK_STATE.STOPPED;
    }

    void OnPauseEvent(PauseEvent ctx)
    {
        if (ctx.isPaused)
        {
            EventBus<SFXSnapshot>.Raise(new SFXSnapshot(2, true));
            return;
        }

        EventBus<SFXSnapshot>.Raise(new SFXSnapshot(2, false));
    }
}
