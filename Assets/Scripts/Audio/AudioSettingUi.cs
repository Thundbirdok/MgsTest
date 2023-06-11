using UnityEngine;

namespace Audio
{
    using UnityEngine.UI;
    
    public class AudioSettingUi : MonoBehaviour
    {
        [SerializeField]
        private AudioSetting setting;
        
        [SerializeField]
        private Slider slider;

        [SerializeField]
        private Toggle toggle;

        private void OnEnable()
        {
            setting.OnVolumeChanged += SettingsVolumeChanged;
            setting.OnIsOnChanged += SettingsIsOnChanged;

            if (setting.IsInitialized)
            {
                Setup();
            }
            else
            {
                setting.OnInitialized += Setup;
            }

            slider.onValueChanged.AddListener(OnSliderValueChanged);
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        private void Setup()
        {
            setting.OnInitialized -= Setup;
            
            toggle.SetIsOnWithoutNotify(setting.IsOn);
            slider.SetValueWithoutNotify(setting.Volume);
        }

        private void SettingsIsOnChanged()
        {
            toggle.SetIsOnWithoutNotify(setting.IsOn);
        }

        private void SettingsVolumeChanged()
        {
            slider.SetValueWithoutNotify(setting.Volume);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            setting.IsOn = isOn;
        }

        private void OnSliderValueChanged(float value)
        {
            setting.Volume = value;
            
            toggle.isOn = setting.Volume > 0;
        }
    }
}
