using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class DialogueStartFX : MonoBehaviour
{
    CanvasGroup canvasGroup;

    public Vector2 BottomRectValues;
    public Vector2 TopRectValues;

    public RectTransform bottomBg;
    public RectTransform topBg;

    public float fadeTime = 1f;

    public LeanTweenType easeType;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    [YarnCommand("start")]
    public void showLineView()
    {
        print("DialogueMove: starting");

        LeanTween.moveY(bottomBg, BottomRectValues.x, fadeTime).setEase(easeType);
        LeanTween.moveY(topBg, TopRectValues.x, fadeTime).setEase(easeType);
    }

    [YarnCommand("end")]
    public void hideLineView()
    {
        print("DialogueMove: closing");

        LeanTween.moveY(bottomBg, BottomRectValues.y, fadeTime).setEase(easeType);
        LeanTween.moveY(topBg, TopRectValues.y, fadeTime).setEase(easeType);
    }
}
