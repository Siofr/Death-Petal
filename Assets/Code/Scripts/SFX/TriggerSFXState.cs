using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using SFXUtil;
using System.Collections.Generic;

public class TriggerSFXState : MonoBehaviour
{
    [System.Serializable]
    public struct AudioReference
    {
        public string audioName;
        public EventReference eventReference;
    }

    public AudioReference[] audioReferences;
    public Dictionary<string, EventInstance> audioBanks = new Dictionary<string, EventInstance>();

    public void Start()
    {
        if (audioReferences.Length <= 0) return;

        foreach(AudioReference reference in audioReferences)
        {
            audioBanks.Add(reference.audioName, SFXUtilities.CreateEventInstance(reference.eventReference, this.gameObject));
        }
    }

    public void OnStateEnter(string stateName)
    {
        if (!audioBanks.ContainsKey(stateName)) return;

        EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(audioBanks[stateName], this.gameObject));
    }

    public void OnStateExit(string stateName)
    {
        if (!audioBanks.ContainsKey(stateName)) return;

        audioBanks[stateName].stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
