using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class ShowCharacters : MonoBehaviour
{
    public Image LeftActorImage;
    public Image RightActorImage;
    
    public Sprite[] CharacterSprites;
    
    [YarnCommand("actor")]
    public void ShowActor(int actorId, bool left = true)
    {
        if (left)
        {
            LeftActorImage.enabled = true;
            LeftActorImage.sprite = CharacterSprites[actorId];
        }
        else
        {
            RightActorImage.enabled = true;
            RightActorImage.sprite = CharacterSprites[actorId];
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
