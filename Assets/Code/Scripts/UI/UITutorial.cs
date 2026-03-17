using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using DG.Tweening;

public class UITutorial : MonoBehaviour
{
    private Transform _tutorialContainer;
    private Dictionary<GameObject, TMP_Text> _tutorialPopups = new Dictionary<GameObject, TMP_Text>();
    private Dictionary<string, GameObject> _tutorialReferences = new Dictionary<string, GameObject>();

    // Events
    private EventBindings<TutorialTriggerEvent> _tutorialTriggerListener;
    private EventBindings<AdvanceTutorialEvent> _advanceTutorialEvent;
    private EventBindings<EndTutorialEvent> _endTutorialEvent;

    // Animation
    private Sequence _animationSequence;
    private float _xStartPos;
    private GameObject _fadeOutObject;
    private Dictionary<string, string> _savedDict;

    private void Awake()
    {
        _tutorialTriggerListener = new EventBindings<TutorialTriggerEvent>(OnTutorialTrigger);
        _advanceTutorialEvent = new EventBindings<AdvanceTutorialEvent>(AdvanceStep);
        _endTutorialEvent = new EventBindings<EndTutorialEvent>(EndTutorial);

        _tutorialContainer = transform.GetChild(0);

        foreach(Transform item in _tutorialContainer)
        {
            _tutorialPopups.Add(item.gameObject, item.GetChild(0).GetComponent<TMP_Text>());
        }

        _xStartPos = _tutorialPopups.ElementAt(0).Key.transform.position.x;
    }

    private void OnEnable()
    {
        EventBus<TutorialTriggerEvent>.Register(_tutorialTriggerListener);
        EventBus<AdvanceTutorialEvent>.Register(_advanceTutorialEvent);
        EventBus<EndTutorialEvent>.Register(_endTutorialEvent);
    }

    private void OnDisable()
    {
        EventBus<TutorialTriggerEvent>.Unregister(_tutorialTriggerListener);
        EventBus<AdvanceTutorialEvent>.Unregister(_advanceTutorialEvent);
        EventBus<EndTutorialEvent>.Unregister(_endTutorialEvent);
    }

    private void AdvanceStep(AdvanceTutorialEvent ctx)
    {
        _animationSequence = DOTween.Sequence();
        AnimateOut(_tutorialReferences[ctx.actionName]);
        _fadeOutObject = _tutorialReferences[ctx.actionName];
        _animationSequence.OnComplete(ResetPosition);
    }

    private void ShowTutorial()
    {
        _animationSequence = DOTween.Sequence();
        _tutorialReferences.Clear();

        for(int i = 0; i < _savedDict.Count;  i++)
        {
            _tutorialPopups.ElementAt(i).Key.SetActive(true);
            AnimateIn(_tutorialPopups.ElementAt(i).Key);
            _tutorialPopups.ElementAt(i).Value.text = _savedDict.ElementAt(i).Key;
            _tutorialReferences.Add(_savedDict.ElementAt(i).Value, _tutorialPopups.ElementAt(i).Key);
        }

        _animationSequence.Play();
    }

    private void OnTutorialTrigger(TutorialTriggerEvent ctx)
    {
        _savedDict = ctx.tutorialSteps;

        if (_animationSequence.IsActive()) _animationSequence.OnComplete(ShowTutorial);
        else ShowTutorial();
    }

    private void EndTutorial()
    {
        for (int i = 0; i < _tutorialPopups.Count; i++)
        {
            _tutorialPopups.ElementAt(i).Key.SetActive(false);
        }

        _tutorialReferences.Clear();
    }

    private void AnimateIn(GameObject go)
    {
        go.GetComponent<CanvasGroup>().alpha = 1f;
        _animationSequence
            .Append(go.transform.DOMoveX(200f, 0.15f))
            .Pause();
    }

    private void AnimateOut(GameObject go)
    {
        _animationSequence
            .Append(
            go.GetComponent<CanvasGroup>()
            .DOFade(0, 0.15f))
            .Play();
    }

    private void ResetPosition()
    {
        _fadeOutObject.transform.position = new Vector3(
            _xStartPos,
            _fadeOutObject.transform.position.y,
            _fadeOutObject.transform.position.z);
    }

    private void TestMethod()
    {

    }
}
