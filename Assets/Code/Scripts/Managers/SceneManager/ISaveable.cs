public interface ISaveable<T>: ISaveable where T : SaveData
{
    public T SaveInfo { get; }
}

public interface ISaveable
{
    public SaveData GetSaveData(LevelData levelData);
    public void LoadSaveData(SaveData levelData);
    public void SaveData();
}