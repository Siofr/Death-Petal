using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD;
using Yarn.Unity;


public class ShowCharacters : MonoBehaviour
{

    public Image LeftActorImage;
    public Image RightActorImage;
    public Animator LeftActorAnimator;
    public Animator RightActorAnimator;
    
    public ActorSprites[] actorReferences;
    
    [YarnCommand("actor")]
    public void ShowActor(int actorId, string emotion = "none", bool left = true)
    {
        StartCoroutine(UpdateAnimation(left));
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

    [YarnCommand("audio")]
    public void PlaySFX(int actorId, string emotion = "none")
    {
        RuntimeManager.PlayOneShot(actorReferences.First(x => x.actorID == actorId).GetAudioCue(emotion.ToLower()));
    }

    public void Focus(bool isLeft, bool isActive)
    {
        if (isLeft)
        {
            LeftActorAnimator.SetBool("Focussed", isActive);
        }
        else
        {
            RightActorAnimator.SetBool("Focussed", isActive);
        }
    }

    private IEnumerator UpdateAnimation(bool isLeft)
    {
        yield return new WaitForFixedUpdate();
        if(isLeft)
            LeftActorAnimator.SetTrigger("Update");
        else RightActorAnimator.SetTrigger("Update");
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
