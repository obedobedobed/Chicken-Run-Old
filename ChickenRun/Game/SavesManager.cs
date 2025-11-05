using System;
using System.IO;

namespace ChickenRun;

public static class SavesManager
{
    public static string savesPath { get; private set; }
    = $"{Environment.SpecialFolder.UserProfile}/.ObedWorks/ChickenRun/";
    public static string saveFileName { get; private set; } = "data";

    public static void Save(SaveData saveData)
    {
        // Creating array for data
        bool[] dataArray = new bool[6];

        // Five is levels count
        for (int i = 0; i < 5; i++)
        {
            dataArray[i] = saveData.completedMaps[i];
        }

        // VSync
        dataArray[5] = saveData.vSyncEnabled;

        // Converting bools to strings
        string[] dataStringArray = new string[dataArray.Length];

        for (int i = 0; i < dataArray.Length; i++)
        {
            dataStringArray[i] = dataArray[i] ? "true" : "false";
        }

        // Saving data to file
        File.WriteAllLines(savesPath + saveFileName, dataStringArray);
    }

    public static SaveData Load()
    {
        // Creating arrays for data
        bool[] dataArray = new bool[6];
        string[] dataStringArray;

        // Reading and converting data
        dataStringArray = File.ReadAllLines(savesPath + saveFileName);

        for (int i = 0; i < dataArray.Length; i++)
        {
            try
            {
                dataArray[i] = dataStringArray[i] == "true";
            }
            catch (Exception)
            {
                dataArray[i] = false;
            }
        }

        // Loading maps
        bool[] completedMaps = new bool[5];

        // Five is levels count
        for (int i = 0; i < 5; i++)
        {
            completedMaps[i] = dataArray[i];
        }

        // Returning save data object
        return new SaveData(completedMaps, dataArray[5]);
    }
}