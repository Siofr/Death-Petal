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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag != "Player") return;

        switch (triggerType)
        {
            case TriggerType.STAGE_START:
                EventBus<OnLevelStartEvent>.Raise(new OnLevelStartEvent(stage));
                break;

            case TriggerType.STAGE_END:
                EventBus<OnLevelEndEvent>.Raise(new OnLevelEndEvent());
                break;
        }
    }
}
