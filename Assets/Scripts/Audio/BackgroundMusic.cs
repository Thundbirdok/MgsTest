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
            SetVolume();
            
            audioSetting.OnVolumeChanged += SetVolume;
        }

        private void OnDisable()
        {
            audioSetting.OnVolumeChanged -= SetVolume;
        }

        private void SetVolume()
        {
            musicAudioSource.volume = audioSetting.Volume;
        }
    }
}
