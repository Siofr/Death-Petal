using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class UITutorial : MonoBehaviour
{
    private Transform _tutorialContainer;
    private Dictionary<GameObject, TMP_Text> _tutorialPopups = new Dictionary<GameObject, TMP_Text>();
    private Dictionary<string, GameObject> _tutorialReferences = new Dictionary<string, GameObject>();

    // Events
    private EventBindings<TutorialTriggerEvent> _tutorialTriggerListener;
    private EventBindings<AdvanceTutorialEvent> _advanceTutorialEvent;
    private EventBindings<EndTutorialEvent> _endTutorialEvent;

    private void Awake()
    {
        _tutorialTriggerListener = new EventBindings<TutorialTriggerEvent>(ShowTutorial);
        _advanceTutorialEvent = new EventBindings<AdvanceTutorialEvent>(AdvanceStep);
        _endTutorialEvent = new EventBindings<EndTutorialEvent>(EndTutorial);

        _tutorialContainer = transform.GetChild(0);

        foreach(Transform item in _tutorialContainer)
        {
            _tutorialPopups.Add(item.gameObject, item.GetChild(0).GetComponent<TMP_Text>());
        }
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
        _tutorialReferences[ctx.actionName].SetActive(false);
    }

    private void ShowTutorial(TutorialTriggerEvent ctx)
    {
        _tutorialReferences.Clear();

        for(int i = 0; i < ctx.tutorialSteps.Count;  i++)
        {
            _tutorialPopups.ElementAt(i).Key.SetActive(true);
            _tutorialPopups.ElementAt(i).Value.text = ctx.tutorialSteps.ElementAt(i).Key;
            _tutorialReferences.Add(ctx.tutorialSteps.ElementAt(i).Value, _tutorialPopups.ElementAt(i).Key);
        }
    }

    private void EndTutorial()
    {
        _tutorialReferences.Clear();
    }
}
