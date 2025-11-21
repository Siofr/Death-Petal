using UnityEngine;

public abstract class SaveData : ScriptableObject
{
    public Vector3 position;

    protected void Save(Vector3 @position)
    {
        this.position = @position;
    }

    protected void Load(Transform refTransform)
    {
        refTransform.position = position;
    }
}