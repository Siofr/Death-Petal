using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorSprites", menuName = "Scriptable Objects/ActorSprites")]
public class ActorSprites : ScriptableObject
{
    public int actorID;
    public Sprite characterDefault;
    public Sprite happy;
    public Sprite sad;
    public Sprite confused;
    public Sprite forlorn;
    public Sprite terror;
    
    public Sprite GetSelectedEmotion(string emotion)
    {
        switch (emotion)
        {
            case "happy":
                return happy;
            case "sad":
                return sad;
            case "confused":
                return confused;
            case "forlorn":
                return forlorn;
            case "terror":
                return terror;
            default:
                return characterDefault;
        }
    }
}


