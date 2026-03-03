using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using Unity.Cinemachine;

public class UIGradeDisplay : MonoBehaviour
{
    [System.Serializable]
    public struct text
    {
        public TMP_Text scoreText;
        public TMP_Text gradeText;
    }

    public CinemachineCamera cam;
    public TMP_Text finalGrade;
    [SerializeField] public text[] gradeTexts;
    public Transform container;
    private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();
    public Transform background;
    public Transform continueButton;
    public Transform continueTarget;
    private Vector3 continueStartPos;
    public Transform _backgroundXTarget;
    private Vector3 _startPos;
    private Sequence _animationSequence;

    EventBindings<DisplayEndUI> displayEndUIEventListener;

    private void Awake()
    {
        _startPos = background.position;
        displayEndUIEventListener = new EventBindings<DisplayEndUI>(OnDisplayUI);

        foreach(Transform transform in container)
        {
            canvasGroups.Add(transform.GetComponent<CanvasGroup>());
        }

        continueStartPos = continueButton.position;
    }

    private void OnEnable()
    {
        EventBus<DisplayEndUI>.Register(displayEndUIEventListener);
    }

    private void OnDisable()
    {
        EventBus<DisplayEndUI>.Unregister(displayEndUIEventListener);
    }

    private void OnDisplayUI(DisplayEndUI ctx)
    {
        cam.transform.gameObject.SetActive(true);

        continueButton.position.Set(
            continueStartPos.x,
            continueStartPos.y,
            continueStartPos.z
            );

        background.position.Set(
            _startPos.x,
            _startPos.y,
            _startPos.z
            );

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

    public void ResetUI()
    {
        cam.transform.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
    }
}
