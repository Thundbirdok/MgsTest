using UnityEngine;

namespace ServerInteractions
{
    using System.IO;
    using Newtonsoft.Json;

    public class ServerAddressSettingSaver : MonoBehaviour
    {
        [SerializeField]
        private ServerAddressSetting setting;
        
        private const string SAVE_FILE_NAME = "config.txt";

        private string _filePath;

        private void OnEnable() => Load();

        private void OnDisable() => Save();

        private void Load()
        {
            SetPath();

            var save = GetSettingsSaves();

            SetupSetting(save);
        }

        private void Save()
        {
            SetPath();
            
            var save = GetSave();
            
            WriteSettingsSaves(save);
        }

        private void SetupSetting(ServerAddressSettingJson save)
        {
            if (save != null)
            {
                setting.Setup(save);

                return;
            }

            setting.SetupDefault();
        }

        private void WriteSettingsSaves(ServerAddressSettingJson save)
        {
            var json = JsonConvert.SerializeObject(save);

            if (File.Exists(_filePath) == false)
            {
                File.Create(_filePath);
            }

            File.WriteAllText(_filePath, json);
        }

        private ServerAddressSettingJson GetSettingsSaves()
        {
            if (File.Exists(_filePath) == false)
            {
                File.Create(_filePath);

                return null;
            }
            
            var json = File.ReadAllText(_filePath);

            if (string.IsNullOrEmpty(json) || json == "{}")
            {
                return null;
            }
            
            return JsonConvert.DeserializeObject<ServerAddressSettingJson>(json);
        }

        private ServerAddressSettingJson GetSave()
        {
            return new ServerAddressSettingJson
            {
                address = setting.Address,
                port = setting.Port
            };
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
