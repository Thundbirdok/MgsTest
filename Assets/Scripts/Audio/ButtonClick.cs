using UnityEngine;

namespace Audio
{
    using UnityEngine.UI;

    public class ButtonClick : MonoBehaviour
    {
        [SerializeField]
        private AudioClip clickClip;
        
        [SerializeField]
        private AudioSource clickSource;

        [SerializeField]
        private AudioSetting setting;

        [SerializeField]
        private Button button; 
        
        private void OnEnable() => button.onClick.AddListener(Click);

        private void OnDisable() => button.onClick.RemoveListener(Click);

        private void Click() => clickSource.PlayOneShot(clickClip, setting.Volume);
    }
}
