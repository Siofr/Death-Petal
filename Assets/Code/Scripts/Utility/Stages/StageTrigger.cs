using UnityEngine;

public class StageTrigger : MonoBehaviour
{

    public enum TriggerType
    {
        STAGE_START,
        STAGE_END,
    }

    public TriggerType triggerType;
    public Stage stage;
    private BoxCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag != "Player") return;

        switch (triggerType)
        {
            case TriggerType.STAGE_START:
                EventBus<OnLevelStartEvent>.Raise(new OnLevelStartEvent(stage, Time.time));
                break;

            case TriggerType.STAGE_END:
                EventBus<OnLevelEndEvent>.Raise(new OnLevelEndEvent(stage));
                break;
        }

        _collider.enabled = false;
    }
}
