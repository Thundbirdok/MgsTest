using UnityEngine;

namespace Audio
{
    public class BackgroundMusic : MonoBehaviour
    {
        [SerializeField]
        private AudioSource musicAudioSource;

        [SerializeField]
        private AudioSetting audioSetting;
        
        private void OnEnable()
        {
            if (audioSetting.IsInitialized)
            {
                SetVolume();
            }

            audioSetting.OnVolumeChanged += SetVolume;
            audioSetting.OnInitialized += SetVolume;
        }

        private void OnDisable()
        {
            audioSetting.OnVolumeChanged -= SetVolume;
            audioSetting.OnInitialized -= SetVolume;
        }

        private void SetVolume() => musicAudioSource.volume = audioSetting.Volume;
    }
}
