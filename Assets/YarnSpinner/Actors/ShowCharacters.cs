using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using FMOD;
using TMPro;
using Yarn.Unity;


public class ShowCharacters : MonoBehaviour
{

    public Image LeftActorImage;
    public Image RightActorImage;
    public Image ItemImage;
    public Animator LeftActorAnimator;
    public Animator RightActorAnimator;
    public Animator ItemImageAnimator;
    public TMP_Text ItemNameField;
    public TMP_Text ItemDescriptionField;
    
    public ActorSprites[] actorReferences;
    public DialogItem[] dialogItems;
    
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

    [YarnCommand("item")]
    public void ShowItem(int itemID, bool setActive)
    {
        ItemImage.gameObject.SetActive(setActive);
        var dialogItem = dialogItems.First(x => x.itemID == itemID);
        ItemImage.sprite = dialogItem.itemIcon;
        ItemImageAnimator.enabled = setActive;
        ItemNameField.text = dialogItem.itemName;
        ItemDescriptionField.text = dialogItem.itemDescription;
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

    public void UnFocus()
    {
        LeftActorAnimator.SetBool("Focussed", false);
        RightActorAnimator.SetBool("Focussed", false);
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
