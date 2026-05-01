using UnityEngine;

public class EntityTrigger : EntityBase, IEntity
{
    public string yarnNode;
    public bool isOneShot;

    public bool isSavePoint;
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        //EventBus<LevelSaveEvent>.Raise(new LevelSaveEvent());
        
        int weaknessCount = Weaknesses.Count;

        print("Weakness before first fail state");
        if (!Weaknesses.Contains(weakness))
            return;
        print("Weakness past first fail state");
        if (weakness.WeakType.HasFlag(damageType))
        {
            EventBus<TriggerDialogueEvent>.Raise(new TriggerDialogueEvent(yarnNode));
        }

        if (!isOneShot) return;

        weakness.RemoveWeakType(damageType);

        if (weakness.WeakType == WeakTypes.NONE)
        {
            Weaknesses.Remove(weakness);
            Destroy(weakness.transform.parent.gameObject);
        }

        if (Weaknesses.Count == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
