using UnityEngine;

public static class SaveSystem
{
    private const string SaveKey = "FireMe_SaveData";

    public static void Save(SaveData saveData)
    {
        if (saveData == null)
        {
            return;
        }

        var json = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        if (!HasSave())
        {
            return null;
        }

        var json = PlayerPrefs.GetString(SaveKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonUtility.FromJson<SaveData>(json);
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }

    public static void DeleteSave()
    {
        if (!HasSave())
        {
            return;
        }

        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}
