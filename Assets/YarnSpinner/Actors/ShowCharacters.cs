using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;


public class ShowCharacters : MonoBehaviour
{

    public Image LeftActorImage;
    public Image RightActorImage;
    
    public ActorSprites[] actorReferences;
    
    [YarnCommand("actor")]
    public void ShowActor(int actorId, string emotion = "none", bool left = true)
    {
        if (left)
        {
            LeftActorImage.enabled = true;
            LeftActorImage.sprite = actorReferences.First(x => x.actorID == actorId).GetSelectedEmotion(emotion.ToLower());
        }
        else
        {
            RightActorImage.enabled = true;
            RightActorImage.sprite = actorReferences.First(x => x.actorID == actorId).GetSelectedEmotion(emotion.ToLower());
        }
    }
    

    [YarnCommand("hideActor")]
    public void HideActor(bool left = true)
    {
        if (left)
        {
            LeftActorImage.enabled = true;
        }
        else
        {
            RightActorImage.enabled = true;
        }
    }

    [YarnCommand("hideAll")]
    public void HideAllActors()
    {
        LeftActorImage.enabled = false;
        RightActorImage.enabled = false;
    }
}
