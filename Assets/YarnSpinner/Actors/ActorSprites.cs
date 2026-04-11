using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActorSprites", menuName = "Scriptable Objects/ActorSprites")]
public class ActorSprites : ScriptableObject
{
    public int actorID;
    public Sprite characterDefault;
    public Sprite happy;
    public Sprite sad;
    public Sprite surprised;
    public Sprite forlorn;
    public Sprite evil;
    
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
}


