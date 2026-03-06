using System.Collections;
using UnityEngine;

public struct ChangeScoreEvent : IEvent
{
    public string text;
    public float score;

    public ChangeScoreEvent(string text, float score)
    {
        this.score = score;
        this.text = text;
    }
}

public struct WipeComboEvent : IEvent
{

}

public class ScoreManager : MonoBehaviour
{
    // Combo Counter Variables
    private float scoreMultiplier = 1.0f;
    private float currentScore;

    private float scoreMultiplierThreshold = 500.0f;
    private float currentScoreThreshold;

    private EventBindings<ChangeScoreEvent> _changeScoreEventListener;
    private EventBindings<WipeComboEvent> _wipeComboEventListener;

    private void Awake()
    {
        _changeScoreEventListener = new EventBindings<ChangeScoreEvent>(OnScoreChange);
        _wipeComboEventListener = new EventBindings<WipeComboEvent>(OnComboWipe);
    }

    private void OnEnable()
    {
        EventBus<ChangeScoreEvent>.Register(_changeScoreEventListener);
        EventBus<WipeComboEvent>.Register(_wipeComboEventListener);
    }

    private void OnDisable()
    {
        EventBus<ChangeScoreEvent>.Unregister(_changeScoreEventListener);
        EventBus<WipeComboEvent>.Register(_wipeComboEventListener);
    }

    void OnScoreChange(ChangeScoreEvent ctx)
    {
        UpdateScore(ctx.score);
    }

    void UpdateScore(float score)
    {
        // What to do when score is changed
        currentScore = currentScore + score * scoreMultiplier;
        currentScoreThreshold += score;

        int multiplierSteps = Mathf.RoundToInt(
            Mathf.Clamp(currentScoreThreshold / scoreMultiplierThreshold, 1.0f, 3.0f / 0.5f)
            );

        if (currentScoreThreshold >= scoreMultiplierThreshold)
        {
            currentScoreThreshold = 0.0f;
            scoreMultiplier = Mathf.Clamp(scoreMultiplier + 0.5f * multiplierSteps, 1.0f, 3.0f);
            EventBus<UpdateComboMultEvent>.Raise(new UpdateComboMultEvent(scoreMultiplier));
        }

        EventBus<UpdateScoreEvent>.Raise(new UpdateScoreEvent(currentScore));
    }

    void OnComboWipe()
    {
        scoreMultiplier = 1.0f;
        currentScoreThreshold = 0;
        EventBus<UpdateComboMultEvent>.Raise(new UpdateComboMultEvent(scoreMultiplier));
    }
}
