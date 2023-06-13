using UnityEngine;

namespace ServerInteractions
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    public class ServerAddressSettingSaver : MonoBehaviour
    {
        [SerializeField]
        private ServerAddressSetting[] settings;
        
        private const string SAVE_FILE_NAME = "config.txt";

        private string _filePath;

        private void OnEnable() => Load();

        private void OnDisable() => Save();

        private void Load()
        {
            SetPath();

            var save = GetSettingsSaves();

            SetupSettings(save);
        }

        private void Save()
        {
            SetPath();
            
            var save = GetSaves();
            
            WriteSettingsSaves(save);
        }

        private void SetupSettings(List<ServerAddressSettingJson> saves)
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

        private List<ServerAddressSettingJson> GetSettingsSaves()
        {
            if (File.Exists(_filePath) == false)
            {
                File.Create(_filePath);

                return new List<ServerAddressSettingJson>();
            }
            
            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrEmpty(json) || json[0] == '{')
            {
                return new List<ServerAddressSettingJson>();
            }
            
            return JsonConvert.DeserializeObject<List<ServerAddressSettingJson>>(json);
        }
        
        private void WriteSettingsSaves(List<ServerAddressSettingJson> saves)
        {
            var updatedJson = JsonConvert.SerializeObject(saves);

            if (File.Exists(_filePath) == false)
            {
                File.Create(_filePath);
            }

            File.WriteAllText(_filePath, updatedJson);
        }

        private List<ServerAddressSettingJson> GetSaves()
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
                _filePath = Path.Combine(Application.streamingAssetsPath, SAVE_FILE_NAME);
            }
        }
    }
}
