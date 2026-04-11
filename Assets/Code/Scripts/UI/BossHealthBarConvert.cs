using UnityEngine;

public class BossHealthBarConvert : MonoBehaviour
{
    public HealthBar bossBar;
    private float relativeHealthValue;

    private int maxWpCount;
    private int currentWpCount;

    private EventBindings<CorrectShotEvent> _correctShotEventListener;

    void Start()
    {
        maxWpCount = GetCurrentWpCount();

        if(bossBar == null)
        {
            Debug.LogError("[BOSSHP] Missing boss bar connection");

            this.enabled = false;
        }
    }

    void OnEnable()
    {
        _correctShotEventListener = new EventBindings<CorrectShotEvent>(OnEnemyShot);
        EventBus<CorrectShotEvent>.Register(_correctShotEventListener);
    }

    void OnDisable()
    {
        EventBus<CorrectShotEvent>.Unregister(_correctShotEventListener);
    }

    void OnEnemyShot(CorrectShotEvent ctx)
    {
        UpdateRelativeHealth();
    }

    void UpdateRelativeHealth()
    {
        currentWpCount = GetCurrentWpCount();
        relativeHealthValue = (float) currentWpCount /  (float) maxWpCount;

        bossBar.BarValue = relativeHealthValue;
    }

    int GetCurrentWpCount()
    {
        return GetComponentsInChildren<Weakness>().Length;
    }
}
