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
    private Gamepad _gamepad;
    private EventBindings<HapticFeedbackEvent> _onHapticEventListener;

    protected override void Awake()
    {
        base.Awake();
        _onHapticEventListener = new EventBindings<HapticFeedbackEvent>(OnHapticEvent);
    }

    private void OnEnable()
    {
        EventBus<HapticFeedbackEvent>.Register(_onHapticEventListener);
    }

    private void OnDisable()
    {
        EventBus<HapticFeedbackEvent>.Unregister(_onHapticEventListener);
    }

    public void OnHapticEvent(HapticFeedbackEvent ctx)
    {
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
}
