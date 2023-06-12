using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace ServerInteractions
{
    public class VideoStreamAddressSaver : MonoBehaviour
    {
        [SerializeField]
        private VideoStreamAddressSetting setting;
            
        private const string SAVE_FILE_NAME = "videoStreamConfig.txt";
    
        private string _filePath;
    
        private void OnEnable() => Load();
    
        private void OnDisable() => Save();
    
        private void Load()
        {
            _filePath = Path.Combine(Application.streamingAssetsPath, SAVE_FILE_NAME);
    
            var save = GetSettingsSaves();
    
            SetupSetting(save);
        }
    
        private void Save()
        {
            var save = GetSave();
                
            WriteSettingsSaves(save);
        }
    
        private void SetupSetting(VideoStreamAddressSettingJson save)
        {
            if (save != null)
            {
                setting.Setup(save);
    
                return;
            }
    
            setting.SetupDefault();
        }
    
        private void WriteSettingsSaves(VideoStreamAddressSettingJson save)
        {
            var json = JsonConvert.SerializeObject(save);
    
            if (File.Exists(_filePath) == false)
            {
                File.Create(_filePath);
            }
    
            File.WriteAllText(_filePath, json);
        }
    
        private VideoStreamAddressSettingJson GetSettingsSaves()
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
                
            return JsonConvert.DeserializeObject<VideoStreamAddressSettingJson>(json);
        }
    
        private VideoStreamAddressSettingJson GetSave()
        {
            return new VideoStreamAddressSettingJson
            {
                address = setting.Address
            };
        }
    }
}
