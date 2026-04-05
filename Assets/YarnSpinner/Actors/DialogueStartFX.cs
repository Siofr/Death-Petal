using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class DialogueStartFX : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public float fadeTime = 1f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    [YarnCommand("start")]
    public void showLineView()
    {
        StartCoroutine(Effects.FadeAlpha(canvasGroup, 0, 1, fadeTime));
    }

    [YarnCommand("end")]
    public void hideLineView()
    {
        StartCoroutine(Effects.FadeAlpha(canvasGroup, 1, 0, fadeTime));
    }
}
