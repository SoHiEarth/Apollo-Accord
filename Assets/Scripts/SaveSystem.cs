using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace SaveSystem
{
  public class SaveData
  {
    public Dictionary<string, string> data = new Dictionary<string, string>();
  }

  public class ConfigurationData
  {
    public Dictionary<string, string> settings = new Dictionary<string, string>();
  }

  public class SaveSystem
  {
    public static void SaveGame(SaveData saveData, string fileName, System.Action callback)
    {
      string content = "";
      foreach (var kvp in saveData.data)
      {
        content += kvp.Key + "=" + kvp.Value + "\n";
      }
      File.WriteAllText(Application.persistentDataPath + "/" + fileName, content);
      callback?.Invoke();
    }

    public static List<string> QuerySavedGames()
    {
      List<string> savedGames = new List<string>();
      string[] files = Directory.GetFiles(Application.persistentDataPath, "*.json");
      foreach (string file in files)
      {
        if (Path.GetFileName(file) != "config.json") // Exclude config file
        {
          savedGames.Add(Path.GetFileNameWithoutExtension(file));
        }
      }
      return savedGames;
    }

    public static SaveData LoadGame(string fileName)
    {
      string filePath = Application.persistentDataPath + "/" + fileName + ".json";
      if (File.Exists(filePath))
      {
        string content = File.ReadAllText(filePath);
        SaveData saveData = new SaveData();
        foreach (string line in content.Split('\n'))
        {
          if (!string.IsNullOrEmpty(line))
          {
            string[] parts = line.Split('=');
            if (parts.Length == 2)
            {
              saveData.data[parts[0]] = parts[1];
            }
          }
        }
        return saveData;
      }
      else
      {
        Debug.LogError("Save file not found: " + filePath);
        return null;
      }
    }

    public static void SaveConfiguration(ConfigurationData configData, System.Action callback)
    {
      string content = "";
      foreach (var kvp in configData.settings)
      {
        content += kvp.Key + "=" + kvp.Value + "\n";
      }
      File.WriteAllText(Application.persistentDataPath + "/config.json", content);
      if (callback != null && File.Exists(Application.persistentDataPath + "/config.json"))
      {
        callback?.Invoke();
      }
    }

    public static ConfigurationData LoadConfiguration()
    {
      string filePath = Application.persistentDataPath + "/config.json";
      if (File.Exists(filePath))
      {
        string content = File.ReadAllText(filePath);
        ConfigurationData configData = new ConfigurationData();
        foreach (string line in content.Split('\n'))
        {
          if (!string.IsNullOrEmpty(line))
          {
            string[] parts = line.Split('=');
            if (parts.Length == 2)
            {
              configData.settings[parts[0]] = parts[1];
            }
          }
        }
        return configData;
      }
      else
      {
        Debug.LogWarning("Configuration file not found: " + filePath);
        return new ConfigurationData(); // Return an empty configuration if not found
      }
    }
  }
}