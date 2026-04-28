using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;
using FMOD;
using Yarn.Unity;

public class DialogueStartFX : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public ShowCharacters characterDisplayController;

    public Vector2 BottomRectValues;
    public Vector2 TopRectValues;

    public RectTransform bottomBg;
    public RectTransform topBg;

    public float fadeTime = 1f;

    public LeanTweenType easeType;

    public EventReference dialogueStartSFX;
    public EventReference dialogueEndSFX;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    [YarnCommand("start")]
    public void showLineView()
    {
        //print("DialogueMove: starting");

        EventBus<SFXSnapshot>.Raise(new SFXSnapshot(1, true));
        RuntimeManager.PlayOneShot(dialogueStartSFX);
        LeanTween.moveY(bottomBg, BottomRectValues.x, fadeTime).setEase(easeType);
        LeanTween.moveY(topBg, TopRectValues.x, fadeTime).setEase(easeType);
    }

    [YarnCommand("end")]
    public void hideLineView()
    {
        //print("DialogueMove: closing");

        EventBus<SFXSnapshot>.Raise(new SFXSnapshot(1, false));
        RuntimeManager.PlayOneShot(dialogueEndSFX);
        LeanTween.moveY(bottomBg, BottomRectValues.y, fadeTime).setEase(easeType);
        LeanTween.moveY(topBg, TopRectValues.y, fadeTime).setEase(easeType);
    }

    
    // Worlds hackiest implementation
    public void Relay(string speaker)
    {
        //print("Message received: " + speaker);
        bool isHarmony = speaker.StartsWith("Harmony");
        characterDisplayController.Focus(isHarmony, true);
        characterDisplayController.Focus(!isHarmony, false);
    }
}
