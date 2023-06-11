using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Audio
{
    using Newtonsoft.Json;

    public class AudioSettingsSaver : MonoBehaviour
    {
        [SerializeField]
        public List<AudioSetting> settings;

        private const string SAVE_FILE_NAME = "AudioSettings.json";

        private string _filePath;

        private void OnEnable() => Load();

        private void OnDisable() => Save();

        private void Load()
        {
            _filePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

            var saves = GetSettingsSaves();

            SetupSettings(saves);
        }

        private void Save()
        {
            var saves = GetSettingsSaves();

            UpdateSaves(ref saves);
            
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
        
        private void UpdateSaves(ref List<AudioSettingJson> saves)
        {
            foreach (var setting in settings)
            {
                var existingSave = saves.Find
                (
                    obj => obj.key == setting.Key
                );

                if (existingSave != null)
                {
                    existingSave.value = setting.Value;
                    existingSave.isOn = setting.IsOn;

                    continue;
                }
                
                var newSave = new AudioSettingJson
                {
                    key = setting.Key,
                    isOn = setting.IsOn,
                    value = setting.Value
                };

                saves.Add(newSave);
            }
        }
    }
}