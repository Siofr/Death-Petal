using State_Machine;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerReticle : MonoBehaviour
{
    private Transform _reticle;

    public Sprite ReticleSprite
    {
        get
        {
            return _reticleSprite;
        }
        set
        {
            _reticleSprite = value;
            _reticleImage.sprite = _reticleSprite;
        }
    }

    private Sprite _reticleSprite;
    private Sprite _defaultReticle;
    private Image _reticleImage;
    private Transform _activeTarget;
    private Camera _cam;

    public EventBindings<ActiveTargetEvent> activeTargetEventListener;
    public EventBindings<NextBulletEvent> nextBulletEventListener;

    public void Awake()
    {
        activeTargetEventListener = new EventBindings<ActiveTargetEvent>(ActivateTarget);
        nextBulletEventListener = new EventBindings<NextBulletEvent>(OnChangeReticle);
    }

    public void OnEnable()
    {
        EventBus<ActiveTargetEvent>.Register(activeTargetEventListener);
        EventBus<NextBulletEvent>.Register(nextBulletEventListener);
    }

    private void OnDisable()
    {
        EventBus<ActiveTargetEvent>.Unregister(activeTargetEventListener);
        EventBus<NextBulletEvent>.Unregister(nextBulletEventListener);
    }

    void Start()
    {
        _cam = Camera.main;
        _reticle = transform.GetChild(0);
        _reticleImage = _reticle.GetComponent<Image>();
        _defaultReticle = _reticleImage.sprite;
    }

    void Update()
    {
        if (_activeTarget)
        {
            Vector3 screenPos = _cam.WorldToScreenPoint(_activeTarget.parent.position);
            _reticle.position = screenPos;
        }
    }

    private void ActivateTarget(ActiveTargetEvent ctx)
    {
        _activeTarget = ctx.activeTarget;

        if (_activeTarget != null)
        {
            _reticle.gameObject.SetActive(true);
            return;
        }

        _reticle.gameObject.SetActive(false);
    }

    private void OnChangeReticle(NextBulletEvent ctx)
    {
        if (ctx.bulletType == null)
        {
            ReticleSprite = _defaultReticle;
            _reticleImage.color = Color.black;
            return;
        }

        ReticleSprite = ctx.bulletType.bulletReticle;
        _reticleImage.color = ctx.bulletType.bulletColor;
    }
}
