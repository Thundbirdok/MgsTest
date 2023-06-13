namespace Ui
{
    using ServerInteractions;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class VideoStreamSettingUi : MonoBehaviour
    {
        [SerializeField]
        private ServerAddressSetting setting;

        [SerializeField]
        private TMP_InputField address;

        [SerializeField]
        private TMP_InputField port;
        
        [SerializeField]
        private Button button;
        
        private void OnEnable()
        {
            button.onClick.AddListener(OnEndEdit);
            
            if (setting.IsInitialized)
            {
                UpdateField();
                
                return;
            }
            
            setting.OnInitialized += UpdateField;
        }

        private void OnDisable() => button.onClick.RemoveListener(OnEndEdit);

        private void UpdateField()
        {
            setting.OnInitialized -= UpdateField;
            
            address.text = setting.Address;
        }

        private void OnEndEdit() => setting.Address = address.text;
    }
}
