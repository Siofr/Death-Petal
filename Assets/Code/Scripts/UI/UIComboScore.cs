using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using DG.Tweening;

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

    public Transform scrollContainer;
    private int currentScoreChangeIndex = 1;
    private int currentTransformChangeIndex;
    private float lastPosition;
    private List<TMP_Text> scoreTextList = new List<TMP_Text>();
    private List<Transform> transformList = new List<Transform>();

    private Tween animationTween;
    private Sequence sequence;
    private float animMoveDistance;
    private bool isMoving;

    void Start()
    {
        foreach(Transform item in scrollContainer)
        {
            scoreTextList.Add(item.GetComponentInChildren<TMP_Text>());
            transformList.Add(item);
        }

        currentScoreChangeIndex = scoreTextList.Count - 1;
        lastPosition = (scrollContainer.GetComponent<RectTransform>().sizeDelta.y - 25) * -1;
        DOTween.Init();
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

    void OnScoreChange(ChangeScoreEvent ctx)
    {
        scoreTextList[currentScoreChangeIndex].text = string.Format("{0:0}", ctx.score);

        TransformAnimation();
    }

    void OnScoreUpdate(UpdateScoreEvent ctx)
    {
        scoreText.text = string.Format("{0:0}", ctx.score);
    }

    void OnMultiplierUpdate(UpdateComboMultEvent ctx)
    {
        multiplierText.text = string.Format("{0:0.0}", ctx.multiplier);
    }

    void TransformAnimation()
    {
        StartCoroutine(NewTransform());
    }

    IEnumerator TransformOperation()
    {
        if (isMoving) yield return animationTween.WaitForCompletion();

        // sequence = DOTween.Sequence();

        animationTween = scrollContainer.DOMoveY(scrollContainer.transform.position.y + 50, 0.05f);

        yield return animationTween.WaitForCompletion();

        currentScoreChangeIndex--;
        if (currentScoreChangeIndex < 0) currentScoreChangeIndex = scoreTextList.Count - 1;

        // RectTransform currentTransform = scrollContainer.GetChild(currentTransformChangeIndex).GetComponent<RectTransform>();
        // currentTransform.localPosition = new Vector3(currentTransform.localPosition.x, lastPosition, currentTransform.localPosition.z);

        currentTransformChangeIndex++;
        if (currentTransformChangeIndex > scoreTextList.Count - 1) currentTransformChangeIndex = 0;

        isMoving = false;
    }

    IEnumerator NewTransform()
    {
        if (isMoving) yield return animationTween.WaitForCompletion();

        // sequence = DOTween.Sequence();

        // animationTween = scrollContainer.DOMoveY(scrollContainer.transform.position.y + 50, 0.05f);

        sequence = DOTween.Sequence();

        yield return sequence.WaitForCompletion();

        currentScoreChangeIndex--;
        if (currentScoreChangeIndex < 0) currentScoreChangeIndex = scoreTextList.Count - 1;

        RectTransform currentTransform = scrollContainer.GetChild(currentTransformChangeIndex).GetComponent<RectTransform>();
        currentTransform.localPosition = new Vector3(currentTransform.localPosition.x, lastPosition, currentTransform.localPosition.z);

        for (int i = 0; i < transformList.Count; i++)
        {
            sequence.Join(transformList[i].DOMoveY(transformList[i].position.y + 50, 0.05f));
        }

        sequence.Play();

        currentTransformChangeIndex++;
        if (currentTransformChangeIndex > scoreTextList.Count - 1) currentTransformChangeIndex = 0;

        isMoving = false;
    }
}
