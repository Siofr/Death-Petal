using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIGradeDisplay : MonoBehaviour
{
    public TMP_Text stageName;

    [SerializeField] public text[] gradeTexts;

    [System.Serializable]
    public struct text
    {
        public TMP_Text scoreText;
        public TMP_Text gradeText;
    }

    public TMP_Text finalGrade;
    private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();

    public Transform container;
    public Transform background;
    public Transform continueButton;
    public Transform continueTarget;
    public Transform _backgroundXTarget;

    private Vector3 continueStartPos;
    private Vector3 _startPos;

    private Sequence _animationSequence;

    EventBindings<DisplayEndUI> displayEndUIEventListener;
    EventBindings<OnLevelEndEvent> levelEndEventListener;

    private void Awake()
    {
        _startPos = background.position;
        displayEndUIEventListener = new EventBindings<DisplayEndUI>(OnDisplayUI);
        levelEndEventListener = new EventBindings<OnLevelEndEvent>(ChangeStageTitle);

        foreach(Transform transform in container)
        {
            canvasGroups.Add(transform.GetComponent<CanvasGroup>());
        }

        continueStartPos = continueButton.localPosition;
    }

    private void OnEnable()
    {
        EventBus<DisplayEndUI>.Register(displayEndUIEventListener);
        EventBus<OnLevelEndEvent>.Register(levelEndEventListener);
    }

    private void OnDisable()
    {
        EventBus<DisplayEndUI>.Unregister(displayEndUIEventListener);
        EventBus<OnLevelEndEvent>.Unregister(levelEndEventListener);
    }

    private void OnDisplayUI(DisplayEndUI ctx)
    {
        EventBus<ChangeCameraState>.Raise(new ChangeCameraState(true));
        EventBus<TriggerDialogueEvent>.Raise(new TriggerDialogueEvent());

        background.gameObject.SetActive(true);

        for (int i = 0; i < ctx.grades.Count; i++)
        {
            canvasGroups[i].alpha = 0;
            gradeTexts[i].scoreText.text = ctx.grades.ElementAt(i).Key.ToString();
            gradeTexts[i].gradeText.text = ctx.grades.ElementAt(i).Value;
        }

        canvasGroups[canvasGroups.Count - 1].alpha = 0;
        finalGrade.text = ctx.finalGrade;

        _animationSequence = DOTween.Sequence();
        _animationSequence
            .Append(background.DOMove(_backgroundXTarget.position, .5f))
            .SetEase(Ease.InSine);

        _animationSequence.OnComplete(ShowGrades);
        continueButton.GetComponent<Button>().Select();
    }

    private void ShowGrades()
    {
        _animationSequence = DOTween.Sequence();

        foreach(CanvasGroup canvasGroup in canvasGroups)
        {
            _animationSequence.Append(canvasGroup.DOFade(1, 0.5f))
                .Pause();
        }

        _animationSequence.Append(continueButton.DOMove(continueTarget.position, 0.15f).SetEase(Ease.InSine))
            .Play();
    }

    public void DEBUG_endLevel()
    {
        var debugStage = GameObject.FindFirstObjectByType<Stage>();
        EventBus<OnLevelEndEvent>.Raise(new OnLevelEndEvent(debugStage));
    }

    void ChangeStageTitle(OnLevelEndEvent ctx)
    {
        stageName.text = ctx.stage.stageName;
    }

    public void GoToMainMenu()
    {
        StartCoroutine(ReturnToMainMenuWithTransition());
    }

    private IEnumerator ReturnToMainMenuWithTransition()
    {
        EventBus<SetTransitionEvent>.Raise(new SetTransitionEvent(true, true));
        yield return new WaitForSeconds(0.5f);
        yield return SceneManager.LoadSceneAsync(0);
    }

    public void ResetUI()
    {
        continueButton.localPosition = new Vector3(
            continueStartPos.x,
            continueStartPos.y,
            continueStartPos.z
        );

        background.position = new Vector3(
            _startPos.x,
            _startPos.y,
            _startPos.z
        );

        EventBus<ExitDialogueEvent>.Raise(new ExitDialogueEvent());
        EventBus<ChangeCameraState>.Raise(new ChangeCameraState(false));
        background.gameObject.SetActive(false);
    }
}
