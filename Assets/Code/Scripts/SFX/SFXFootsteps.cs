using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SFXFootsteps : MonoBehaviour
{
    public string eventPath;
    private PARAMETER_ID _terrainParam;
    private PARAMETER_ID _walkParam;

    private void Start()
    {
        _walkParam = AssignParamID("WalkRun");
        _terrainParam = AssignParamID("Terrain");
    }

    public void PlayWalkSFX()
    {
        PlayFootstep(0);
    }

    public void PlayRunSFX()
    {
        PlayFootstep(1);
    }

    void PlayFootstep(int motionState)
    {
        EventInstance Footstep = RuntimeManager.CreateInstance(eventPath);
        RuntimeManager.AttachInstanceToGameObject(Footstep, this.gameObject);

        Footstep.setParameterByID(_walkParam, motionState, false);

        Footstep.start();
        Footstep.release();
    }

    private PARAMETER_ID AssignParamID(string parameterName)
    {
        EventDescription eventDescription = RuntimeManager.GetEventDescription(eventPath);
        PARAMETER_DESCRIPTION paramDescription;
        eventDescription.getParameterDescriptionByName(parameterName, out paramDescription);

        return paramDescription.id;
    }
}
