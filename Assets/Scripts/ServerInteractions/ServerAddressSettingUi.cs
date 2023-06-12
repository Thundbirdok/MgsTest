using UnityEngine;

namespace ServerInteractions
{
    using TMPro;

    public class ServerAddressSettingUi : MonoBehaviour
    {
        [SerializeField]
        private ServerAddressSetting setting;

        [SerializeField]
        private TMP_InputField addressField;
        
        [SerializeField]
        private TMP_InputField portField;
        
        private void OnEnable()
        {
            addressField.onEndEdit.AddListener(OnEndEditAddress);
            portField.onEndEdit.AddListener(OnEndEditPort);
            
            if (setting.IsInitialized)
            {
                UpdateFields();
                
                return;
            }
            
            setting.OnInitialized += UpdateFields;
        }

        private void OnDisable()
        {
            addressField.onEndEdit.RemoveListener(OnEndEditAddress);
            portField.onEndEdit.RemoveListener(OnEndEditPort);
        }

        private void UpdateFields()
        {
            setting.OnInitialized -= UpdateFields;
            
            addressField.text = setting.Address;
            portField.text = setting.Port;
        }

        private void OnEndEditAddress(string text)
        {
            setting.Address = text;
        }
        
        private void OnEndEditPort(string text)
        {
            setting.Port = text;
        }
    }
}
