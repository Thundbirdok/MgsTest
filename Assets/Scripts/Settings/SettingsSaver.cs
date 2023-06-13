using UnityEngine;

namespace Settings
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq; 
    using Newtonsoft.Json;

    public class SettingsSaver<T, TU> : MonoBehaviour where T : Setting<TU> where TU : SettingJson
    {
        [SerializeField]
        private List<T> settings;
        
        [SerializeField]
        private string saveFileName = "AudioSettings.json";

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

        private void SetupSettings(List<TU> saves)
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
        
        private void WriteSettingsSaves(List<TU> saves)
        {
            var updatedJson = JsonConvert.SerializeObject(saves);

            if (File.Exists(_filePath) == false)
            {
                File.Create(_filePath);
            }

            File.WriteAllText(_filePath, updatedJson);
        }

        private List<TU> GetSettingsSaves()
        {
            if (File.Exists(_filePath) == false)
            {
                File.Create(_filePath);

                return new List<TU>();
            }
            
            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrEmpty(json) || json == "{}")
            {
                return new List<TU>();
            }
            
            return JsonConvert.DeserializeObject<List<TU>>(json);
        }
        
        private List<TU> GetSaves()
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
                _filePath = Path.Combine(Application.persistentDataPath, saveFileName);
            }
        }
    }
}
