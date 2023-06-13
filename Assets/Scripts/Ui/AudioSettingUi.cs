namespace Ui
{
    using Audio;
    using UnityEngine;
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
            CallSetup();

            Subscribe();
        }

        private void OnDisable() => Unsubscribe();

        private void CallSetup()
        {
            if (setting.IsInitialized)
            {
                Setup();
                
                return;
            }
            
            setting.OnInitialized += Setup;
        }

        private void Subscribe()
        {
            setting.OnVolumeChanged += SettingsVolumeChanged;
            setting.OnIsOnChanged += SettingsIsOnChanged;
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void Unsubscribe()
        {
            setting.OnVolumeChanged -= SettingsVolumeChanged;
            setting.OnIsOnChanged -= SettingsIsOnChanged;
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
            toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        private void Setup()
        {
            setting.OnInitialized -= Setup;
            
            toggle.SetIsOnWithoutNotify(setting.IsOn);
            slider.SetValueWithoutNotify(setting.Volume);
        }

        private void SettingsIsOnChanged() => toggle.SetIsOnWithoutNotify(setting.IsOn);

        private void SettingsVolumeChanged() => slider.SetValueWithoutNotify(setting.Volume);

        private void OnToggleValueChanged(bool isOn) => setting.IsOn = isOn;

        private void OnSliderValueChanged(float value)
        {
            setting.Volume = value;
            
            toggle.isOn = value > 0;
        }
    }
}
