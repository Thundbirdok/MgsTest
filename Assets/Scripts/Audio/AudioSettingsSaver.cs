using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Audio
{
    using System.Linq;
    using Newtonsoft.Json;

    public class AudioSettingsSaver : MonoBehaviour
    {
        [SerializeField]
        private List<AudioSetting> settings;

        private const string SAVE_FILE_NAME = "AudioSettings.json";

        private string _filePath;
        
        private void OnEnable() => Load();

        private void OnDisable() => Save();

        private void Load()
        {
            SetPath();

            var saves = GetSettingsSaves();

            SetupSettings(saves);
        }

        private void Save()
        {
            SetPath();
            
            var saves = GetSaves();
            
            WriteSettingsSaves(saves);
        }

        private void SetupSettings(List<AudioSettingJson> saves)
        {
            foreach (var setting in settings)
            {
                var save = saves.Find(obj => obj.key == setting.Key);
                
                if (save != null)
                {
                    setting.Setup(save);
                    
                    continue;
                }
                
                setting.SetupDefault();
            }
        }
        
        private void WriteSettingsSaves(List<AudioSettingJson> saves)
        {
            var updatedJson = JsonConvert.SerializeObject(saves);

            if (File.Exists(_filePath) == false)
            {
                File.Create(_filePath);
            }

            File.WriteAllText(_filePath, updatedJson);
        }

        private List<AudioSettingJson> GetSettingsSaves()
        {
            if (File.Exists(_filePath) == false)
            {
                File.Create(_filePath);

                return new List<AudioSettingJson>();
            }
            
            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrEmpty(json) || json == "{}")
            {
                return new List<AudioSettingJson>();
            }
            
            return JsonConvert.DeserializeObject<List<AudioSettingJson>>(json);
        }
        
        private List<AudioSettingJson> GetSaves()
        {
            return settings.Select
            (
                setting => setting.GetJson()
            )
            .ToList();
        }

        private void SetPath()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                _filePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            }
        }
    }
}