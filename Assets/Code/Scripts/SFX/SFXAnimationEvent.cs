using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using SFXUtil;
using System.Collections.Generic;

public class SFXAnimationEvent : MonoBehaviour
{
    [System.Serializable]
    public struct AudioReference
    {
        public string audioName;
        public EventReference eventReference;
    }

    public AudioReference[] audioReferences;
    public Dictionary<string, EventInstance> audioBanks = new Dictionary<string, EventInstance>();

    void Awake()
    {
        if (audioReferences.Length <= 0) return;

        foreach (AudioReference reference in audioReferences)
        {
            audioBanks.Add(reference.audioName, SFXUtilities.CreateEventInstance(reference.eventReference, this.gameObject));
        }
    }

    public void PlayAudioEvent(string eventName)
    {
        Debug.Log("Try play Audio Event: " + eventName);

        if (!audioBanks.ContainsKey(eventName)) return;

        Debug.Log("Play Audio Event: " + eventName);
        EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(audioBanks[eventName], this.gameObject));
    }
}
