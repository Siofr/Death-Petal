using UnityEngine;
using FMOD.Studio;
using FMODUnity;

namespace SFXUtil
{
    public static class SFXUtilities
    {
        public static PARAMETER_ID AssignParamID(string parameterName, EventReference eventPath)
        {
            EventDescription eventDescription = RuntimeManager.GetEventDescription(eventPath);
            PARAMETER_DESCRIPTION paramDescription;
            eventDescription.getParameterDescriptionByName(parameterName, out paramDescription);

            return paramDescription.id;
        }

        public static EventInstance  CreateEventInstance(EventReference eventPath, GameObject sourceObject)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventPath);
            RuntimeManager.AttachInstanceToGameObject(eventInstance, sourceObject);
            return eventInstance;
        }
    }
}

