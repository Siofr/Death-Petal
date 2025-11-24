using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using SFXUtil;

public class SFXFootsteps : MonoBehaviour
{
    public EventReference eventPath;
    private EventInstance eventInstance;

    private PARAMETER_ID _terrainParam;
    private PARAMETER_ID _walkParam;

    private void Start()
    {
        _walkParam = SFXUtilities.AssignParamID("WalkRun", eventPath);
        _terrainParam = SFXUtilities.AssignParamID("Terrain", eventPath);

        eventInstance = SFXUtilities.CreateEventInstance(eventPath, this.gameObject);
    }

    public void PlayWalkSFX()
    {
        EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(eventInstance, this.gameObject));
    }
}
