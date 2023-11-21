using SharpConfig;
using System;
using System.IO;
using UnityEngine;

internal class ConfigManager : MonoBehaviour
{
    [SerializeField]
    private string configFileName = "config.cfg";

    private ConfigurableMonoBehaviour[] configurableObjects;

    internal static ConfigManager manager;
    private Configuration cfg;

    public Configuration GetConfig { get => cfg; }

    private void Awake()
    {
        configurableObjects = (ConfigurableMonoBehaviour[])FindObjectsOfType(typeof(ConfigurableMonoBehaviour));
        cfg = new Configuration();

        try
        {
            string path = Path.Combine(Application.persistentDataPath, configFileName);

            if (!File.Exists(path))
            {
                SetupCleanConfiguration();
                SaveConfig();
            }
            cfg = Configuration.LoadFromFile(path);

            LoadConfiguration();
            SaveConfig();
        }
        catch(Exception e)
        {
            Debug.LogException(e);
            cfg = new Configuration();
            SetupCleanConfiguration();
        }

        manager = this;
    }

    private void SetupCleanConfiguration()
    {
        if (configurableObjects != null && configurableObjects.Length > 0)
        {
            foreach (ConfigurableMonoBehaviour configurableObject in configurableObjects)
            {
                configurableObject.SetupCleanConfiguration(cfg);
            }
        }
    }

    public void LoadConfiguration()
    {
        if (configurableObjects != null && configurableObjects.Length > 0)
        {
            foreach (ConfigurableMonoBehaviour configurableObject in configurableObjects)
            {
                configurableObject.LoadConfiguration(cfg);
            }
        }
    }

    private void SaveConfig()
    {
        Debug.Log("[ConfigManager]: Saving config...");
        string path = Path.Combine(Application.persistentDataPath, configFileName);
        Debug.Log(path);
        cfg.SaveToFile(path);
    }

    public void SaveAllConfig()
    {
        SetupCleanConfiguration();
        SaveConfig();
    }
}