namespace ChickenRun;

// Loaded data / data to save
public class SaveData
{
    public bool[] completedMaps { get; private set; }
    public bool vSyncEnabled { get; private set; }

    public SaveData(bool[] completedMaps, bool vSyncEnabled)
    {
        this.completedMaps = completedMaps;
        this.vSyncEnabled = vSyncEnabled;
    }
}