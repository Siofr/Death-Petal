public interface ISaveable<T>: ISaveable where T : SaveData
{
    public T SaveInfo { get; }
}

public interface ISaveable
{
    public SaveData GetSaveInfo();
    public void LoadData(SaveData saveData);
    public void SaveData();
}