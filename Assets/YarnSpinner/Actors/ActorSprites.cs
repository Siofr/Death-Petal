using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;

[CreateAssetMenu(fileName = "ActorSprites", menuName = "Scriptable Objects/ActorSprites")]
public class ActorSprites : ScriptableObject
{
    public int actorID;
    [Header("Default Reactions")]
    public Sprite characterDefault;
    public EventReference defaultSFX;
    [Header("Happy Reactions")]
    public Sprite happy;
    public EventReference happySFX;
    [Header("Sad Reactions")]
    public Sprite sad;
    public EventReference sadSFX;
    [Header("Surprised Reactions")]
    public Sprite surprised;
    public EventReference surprisedSFX;
    [Header("Forlorn Reactions")]
    public Sprite forlorn;
    public EventReference forlornSFX;
    [Header("Evil Reactions")]
    public Sprite evil;
    public EventReference evilSFX;
    
    public Sprite GetSelectedEmotion(string emotion)
    {
        switch (emotion)
        {
            case "happy":
                return happy;
            case "sad":
                return sad;
            case "surprised":
                return surprised;
            case "forlorn":
                return forlorn;
            case "evil":
                return evil;
            default:
                return characterDefault;
        }
    }

    public EventReference GetAudioCue(string emotion)
    {
        switch (emotion)
        {
            case "happy":
                return happySFX;
            case "sad":
                return sadSFX;
            case "surprised":
                return surprisedSFX;
            case "forlorn":
                return forlornSFX;
            case "evil":
                return evilSFX;
            default:
                return defaultSFX;
        }
    }
}


