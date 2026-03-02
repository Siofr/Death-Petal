using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine.SocialPlatforms.Impl;
using System.Linq;

struct UpdateComboMultEvent : IEvent
{
    public float multiplier;

    public UpdateComboMultEvent(float multiplier)
    {
        this.multiplier = multiplier;
    }
}

struct UpdateScoreEvent : IEvent
{
    public float score;

    public UpdateScoreEvent(float score)
    {
        this.score = score;
    }
}

public class UIComboScore : MonoBehaviour
{
    private EventBindings<UpdateScoreEvent> _updateScoreEventListener;
    private EventBindings<UpdateComboMultEvent> _updateComboMultEventListener;
    private EventBindings<ChangeScoreEvent> _changeScoreEventListener;

    public TMP_Text scoreText;
    public TMP_Text multiplierText;

    public Transform scoreContainer;
    public Transform animationTarget;

    private Sequence animationSequence;

    // New Animation Variables
    public Transform scoreContainers;
    private int currentIndex;
    private Dictionary<Transform, TMP_Text> dict = new Dictionary<Transform, TMP_Text>();
    public CanvasGroup canvasGroup;
    private float fadeMultiplier;
    private Vector3 startPos;

    void Start()
    {
        DOTween.Init();

        canvasGroup.alpha = 0f;

        foreach (Transform item in scoreContainers)
        {
            dict.Add(item, item.GetComponentInChildren<TMP_Text>());
        }

        startPos = scoreContainer.GetChild(0).position;
    }

    void Awake()
    {
        _updateComboMultEventListener = new EventBindings<UpdateComboMultEvent>(OnMultiplierUpdate);
        _updateScoreEventListener = new EventBindings<UpdateScoreEvent>(OnScoreUpdate);
        _changeScoreEventListener = new EventBindings<ChangeScoreEvent>(OnScoreChange);
    }

    private void OnEnable()
    {
        EventBus<UpdateComboMultEvent>.Register(_updateComboMultEventListener);
        EventBus<UpdateScoreEvent>.Register(_updateScoreEventListener);
        EventBus<ChangeScoreEvent>.Register(_changeScoreEventListener);
    }

    private void OnDisable()
    {
        EventBus<UpdateComboMultEvent>.Unregister(_updateComboMultEventListener);
        EventBus<UpdateScoreEvent>.Unregister(_updateScoreEventListener);
        EventBus<ChangeScoreEvent>.Unregister(_changeScoreEventListener);
    }

    private void Update()
    {
        fadeMultiplier += Time.deltaTime / 60;
        canvasGroup.alpha = canvasGroup.alpha - 0.05f * fadeMultiplier;
    }

    void OnScoreChange(ChangeScoreEvent ctx)
    {
        if (canvasGroup.alpha <= 0) ResetPosition();

        canvasGroup.alpha = 1.0f;
        fadeMultiplier = 0;
        StartCoroutine(NewAnimation(currentIndex, ctx.text));
        currentIndex = Mathf.Clamp(currentIndex + 1, 0, dict.Count - 1);
    }

    void OnScoreUpdate(UpdateScoreEvent ctx)
    {
        scoreText.text = string.Format("{0:0}", ctx.score);
    }

    void OnMultiplierUpdate(UpdateComboMultEvent ctx)
    {
        multiplierText.text = string.Format("{0:0.0}", ctx.multiplier);
    }

    IEnumerator NewAnimation(int currentIndex, string text)
    {
        Transform newTransform = dict.ElementAt(currentIndex).Key;

        if (currentIndex != dict.Count - 1)
        {
            dict[newTransform].text = string.Format("<allcaps>{0}</allcaps>", text);
        }

        animationSequence = DOTween.Sequence();

        animationSequence
            .Append(newTransform.DOMoveX(animationTarget.position.x, 0.25f));

        yield return null;
    }

    private void ResetPosition()
    {
        currentIndex = 0;

        foreach(var item in dict)
        {
            item.Key.position = new Vector3(
                startPos.x,
                item.Key.position.y,
                item.Key.position.z
                );
        }
    }
}
