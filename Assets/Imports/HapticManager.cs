using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public struct HapticFeedbackEvent : IEvent
{
    public float lowFreqRumble;
    public float highFreqRumble;
    public float duration;

    public HapticFeedbackEvent(float lowFreqRumble,  float highFreqRumble, float duration)
    {
        this.lowFreqRumble = lowFreqRumble;
        this.highFreqRumble = highFreqRumble;
        this.duration = duration;
    }
}

public class HapticManager : Singleton<HapticManager>
{
    private static bool isToggled;
    private Gamepad _gamepad;
    private EventBindings<HapticFeedbackEvent> _onHapticEventListener;

    private EventBindings<ToggleHaptics> _onHapticsToggled;

    protected override void Awake()
    {
        base.Awake();
        _onHapticEventListener = new EventBindings<HapticFeedbackEvent>(OnHapticEvent);
        _onHapticsToggled = new EventBindings<ToggleHaptics>(ToggleHaptics);
    }

    private void OnLevelWasLoaded()
    {
        StopAllCoroutines();
        InputSystem.ResetHaptics();
    }

    private void OnApplicationQuit()
    {
        InputSystem.ResetHaptics();
    }

    private void OnEnable()
    {
        EventBus<HapticFeedbackEvent>.Register(_onHapticEventListener);
        EventBus<ToggleHaptics>.Register(_onHapticsToggled);
    }

    private void OnDisable()
    {
        EventBus<HapticFeedbackEvent>.Unregister(_onHapticEventListener);
        EventBus<ToggleHaptics>.Unregister(_onHapticsToggled);
        InputSystem.ResetHaptics();
    }

    public void OnHapticEvent(HapticFeedbackEvent ctx)
    {
        if (isToggled) return;

        _gamepad = Gamepad.current;

        if (_gamepad == null) return;

        _gamepad.SetMotorSpeeds(ctx.lowFreqRumble, ctx.highFreqRumble);
        StartCoroutine(StopFeedback(ctx.duration));
    }

    private IEnumerator StopFeedback(float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _gamepad.SetMotorSpeeds(0f, 0f);
    }

    public void ToggleHaptics()
    {
        isToggled = !isToggled;
    }
}
