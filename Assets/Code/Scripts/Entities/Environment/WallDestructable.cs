using UnityEngine;

public class WallDestructable: EntityBase
{
    [Header("Wall Fields")]
    [SerializeField] private bool _isHidden;

    private EventBindings<ActiveTargetEvent> _onTargetListener;

    protected override void Awake()
    {
        base.Awake();

        if (_isHidden)
        {
            _onTargetListener = new EventBindings<ActiveTargetEvent>(OnTargeted);
        }
    }

    private void OnEnable()
    {
        if(_isHidden) EventBus<ActiveTargetEvent>.Register(_onTargetListener);
    }

    private void OnDisable()
    {
        if(_isHidden) EventBus<ActiveTargetEvent>.Unregister(_onTargetListener);
    }
    
    private void OnTargeted(ActiveTargetEvent context)
    {
        if (!_isHidden) return;

        if (context.activeTarget == null)
        {
            ToggleAllWeaknessIcons(false);
            return;
        }
        
        foreach (var weakness in Weaknesses)
        {
            if (weakness.transform == context.activeTarget)
            {
                weakness.ToggleIcon(true);
                continue;
            }
            
            weakness.ToggleIcon(false);
        }
    }
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        if (!Weaknesses.Contains(weakness)) return;

        if (weakness.WeakType != damageType)
        {
            EventBus<WrongShotEvent>.Raise(new WrongShotEvent());
            return;
        }
        
        Destroy(gameObject);
    }
}