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

    public Transform scoreContainer;
    public Transform animationTarget;

    private Sequence animationSequence;
    private bool isMoving;
    private Transform scoreTransform;
    private Vector3 startPosition;
    private TMP_Text tmpText;
    private Queue<IEnumerator> scoreQueue = new Queue<IEnumerator>();

    void Start()
    {
        scoreTransform = scoreContainer.transform;
        tmpText = scoreContainer.GetComponentInChildren<TMP_Text>();
        startPosition = scoreTransform.position;

        DOTween.Init();

        StartCoroutine(CoroutineQueue());
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
        scoreQueue.Enqueue(ScoreTween(ctx.score));
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
    }

    IEnumerator CoroutineQueue()
    {
        while (true)
        {
            while (scoreQueue.Count > 0)
            {
                yield return StartCoroutine(scoreQueue.Dequeue());
            }

            yield return null;
        }
    }

    IEnumerator ScoreTween(float score)
    {
        if (isMoving) yield return animationSequence.WaitForCompletion();

        isMoving = true;

        tmpText.text = string.Format("{0:0}", score);

        animationSequence = DOTween.Sequence();

        animationSequence
            .Append(scoreTransform.DOMoveX(animationTarget.position.x, 0.25f))
            .Append(scoreTransform.DOMoveY(scoreTransform.position.y + 100, 0.25f));

        yield return animationSequence.WaitForCompletion();

        scoreTransform.position = startPosition;

        isMoving = false;
    }
}
