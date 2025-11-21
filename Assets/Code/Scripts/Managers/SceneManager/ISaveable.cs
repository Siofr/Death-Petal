public interface ISaveable<T>: ISaveable where T : SaveData
{
    public T SaveInfo { get; }
}

public interface ISaveable
{
    public void LoadSaveData();
    public void SaveData();
}