using UnityEngine;
using DG.Tweening;
using TMPro;

public class UIStageTitle : MonoBehaviour
{
    private EventBindings<OnLevelStartEvent> _onLevelStartEventListener;
    private Vector3 _startPosition;
    public Transform targetPosition;
    private Sequence _animationSequence;
    private Transform _titleTransform;
    private Transform _nameTransform;
    private CanvasGroup _canvasGroup;
    private TMP_Text _stageNameText;

    private void Awake()
    {
        _onLevelStartEventListener = new EventBindings<OnLevelStartEvent>(OnStageStart);
    }

    private void OnEnable()
    {
        EventBus<OnLevelStartEvent>.Register(_onLevelStartEventListener);
    }

    private void OnDisable()
    {
        EventBus<OnLevelStartEvent>.Unregister(_onLevelStartEventListener);
    }

    void Start()
    {
        _titleTransform = transform.GetChild(0);
        _nameTransform = transform.GetChild(1);

        _startPosition = _titleTransform.position;
        _stageNameText = _nameTransform.GetComponent<TMP_Text>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnStageStart(OnLevelStartEvent ctx)
    {
        TitleAnimation(ctx.stage.stageName);
    }

    void TitleAnimation(string stageName)
    {
        _stageNameText.text = stageName;

        _canvasGroup.alpha = 1;
        _animationSequence = DOTween.Sequence();

        _animationSequence
            .Append(_titleTransform.DOMoveX(targetPosition.position.x, 0.1f)).SetEase(Ease.InSine)
            .Append(_nameTransform.DOMoveX(targetPosition.position.x, 0.1f)).SetEase(Ease.InSine)
            .Append(_canvasGroup.DOFade(0, 4.5f)).SetEase(Ease.InSine);

        _animationSequence.OnComplete(ResetPosition);
    }

    private void ResetPosition()
    {
        _titleTransform.position = new Vector3(_startPosition.x,
            _titleTransform.position.y,
            _titleTransform.position.z);

        _nameTransform.position = new Vector3(_startPosition.x,
            _nameTransform.position.y,
            _nameTransform.position.z);
    }
}
