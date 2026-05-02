using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using DG.Tweening;

public class UITutorial : MonoBehaviour
{
    private Transform _tutorialContainer;
    private Transform _destinationTransform;
    private Dictionary<GameObject, TMP_Text> _tutorialPopups = new Dictionary<GameObject, TMP_Text>();
    private Dictionary<string, GameObject> _tutorialReferences = new Dictionary<string, GameObject>();

    // Events
    private EventBindings<TutorialTriggerEvent> _tutorialTriggerListener;
    private EventBindings<AdvanceTutorialEvent> _advanceTutorialEvent;
    private EventBindings<EndTutorialEvent> _endTutorialEvent;

    // Animation
    private Sequence _animationSequence;
    private float _yDestPos;
    private float _yStartPos;
    private GameObject _fadeOutObject;
    private Dictionary<string, string> _savedDict;

    private string _nextAction;
    private int _currentStep = 0;

    private void Awake()
    {
        _destinationTransform = transform.GetChild(1).transform;
        _yDestPos = _destinationTransform.position.y;

        _tutorialTriggerListener = new EventBindings<TutorialTriggerEvent>(ResetPosition);
        _advanceTutorialEvent = new EventBindings<AdvanceTutorialEvent>(AdvanceStep);
        _endTutorialEvent = new EventBindings<EndTutorialEvent>(EndTutorial);

        _tutorialContainer = transform.GetChild(0);

        foreach(Transform item in _tutorialContainer)
        {
            _tutorialPopups.Add(item.gameObject, item.GetChild(1).GetComponent<TMP_Text>());
        }

        _yStartPos = _tutorialPopups.ElementAt(0).Key.transform.position.y;
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
        // AnimateOut(_tutorialReferences[ctx.actionName]);

        _currentStep++;

        if (_currentStep >= _savedDict.Count)
        {
            _currentStep = 0;
            return;
        }

        ShowTutorial();
        // _fadeOutObject = _tutorialReferences[ctx.actionName];
        // _animationSequence.OnComplete(ResetPosition);
    }

    private void ShowTutorial()
    {
        _animationSequence = DOTween.Sequence();

        _tutorialPopups.ElementAt(0).Key.SetActive(true);
        Vector3 resetPos = _tutorialPopups.ElementAt(0).Key.transform.position;

        _tutorialPopups.ElementAt(0).Key.transform.position = new Vector3(
            resetPos.x,
            _yDestPos,
            resetPos.z
            );

        AnimateIn(_tutorialPopups.ElementAt(0).Key);
        _tutorialPopups.ElementAt(0).Value.text = _savedDict.ElementAt(_currentStep).Key;
        _nextAction = _savedDict.ElementAt(_currentStep).Key;
        // _tutorialReferences.Add(_savedDict.ElementAt(_currentStep).Value, _tutorialPopups.ElementAt(0).Key);

        _animationSequence.Play();
    }

    private void OnTutorialTrigger(TutorialTriggerEvent ctx)
    {
        _savedDict = ctx.tutorialSteps;

        if (_animationSequence.IsActive())
        {
            _animationSequence.OnComplete(ShowTutorial);
            Debug.Log("Animating");
        }
        else
        {
            ShowTutorial();
            Debug.Log("Not animating");
        }
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
            .Append(go.transform.DOMoveY(_yDestPos, 0.5f))
            .Pause();
    }

    private void AnimateOut(GameObject go)
    {
        _animationSequence
            .Append(
            go.GetComponent<CanvasGroup>()
            .DOFade(_yStartPos, 0.5f))
            .Play();
        _fadeOutObject = go;
    }

    private void ResetPosition(TutorialTriggerEvent ctx)
    {
        foreach(var item in _tutorialPopups)
        {
            item.Key.transform.position = new Vector3(
                item.Key.transform.position.x,
                _yStartPos,
                item.Key.transform.position.z);
        }

        _savedDict = ctx.tutorialSteps;

        if (_animationSequence.IsActive())
        {
            _animationSequence.OnComplete(ShowTutorial);
            Debug.Log("Animating Tutorial");
        }
        else
        {
            ShowTutorial();
            Debug.Log("Not animating Tutorial");
        }
    }
}
